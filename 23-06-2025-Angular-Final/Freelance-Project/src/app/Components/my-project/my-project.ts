import { Component, OnInit, signal, computed } from '@angular/core';
import { Store } from '@ngrx/store';
import { ProjectService } from '../../Services/project.service';
import { AuthenticationService } from '../../Services/auth.service';
import { CreateProjectModel, ProjectModel } from '../../Models/Project.model';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';
import { ToastrService } from 'ngx-toastr';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { DurationFormatPipe } from '../../Pipes/duration-format.pipe';
import { ConvertTimeSpan } from '../../Misc/ConvertTimeSpan';
import { TimespanToReadablePipe } from '../../Pipes/timespan-to-readable.pipe';
import { FreelancerService } from '../../Services/freelancer.service';
import * as ProjectActions from '../../NgRx/Project/project.actions';
import { selectProjectError, selectAllProjects } from '../../NgRx/Project/project.selector';
import { Subscription } from 'rxjs';
import { Actions, ofType } from '@ngrx/effects';
import { ProjectProposalService } from '../../Services/projectProposal.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-my-project',
  imports: [CommonModule, NgClass, FormsModule, ReactiveFormsModule, DurationFormatPipe, TimespanToReadablePipe],
  templateUrl: './my-project.html',
  styleUrl: './my-project.css'
})
export class MyProject implements OnInit {
  projects = signal<ProjectModel[]>([]);
  selectedProject = signal<ProjectModel | null>(null);
  showCreateProjectModal = signal<boolean>(false);
  showEditProjectModal = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  user = signal<ClientModel | FreelancerModel | null>(null);
  role = computed(() => this.user() ? ('hourlyRate' in this.user()!) ? 'freelancer' : 'client' : null);
  freelancerUsername = signal<string | null>(null);
  statusOptions = ['Completed', 'Pending', 'In Progress', 'Cancelled'];
  
  createProjectForm!: FormGroup;
  editProjectForm!: FormGroup;

  currentPage = signal<number>(1);
  pageSize = 9;
  totalPages = signal<number>(1);
  pagination: any = null;
  searchTerm = signal<string>('');
  statusFilter = signal<string>('');

  
  private subscriptions: Subscription = new Subscription();
  markAsCompleteLoading = signal<boolean>(false);
  cancelProjectLoading = signal<boolean>(false);

  filteredProjects = computed(() => { // Applying filters and search term only on fetched projects(not call API on search!)
    let filtered = this.projects();
    const term = this.searchTerm().trim().toLowerCase();
    if (term) {
      filtered = filtered.filter(p =>
        p.title.toLowerCase().includes(term) ||
        p.description.toLowerCase().includes(term)
      );
    }
    if (this.statusFilter()) {
      filtered = filtered.filter(p => p.status === this.statusFilter());
    }
    return filtered;
  });

  constructor(
    private store: Store,
    private actions$: Actions,
    private fb: FormBuilder,
    private toastr: ToastrService,
    private authService: AuthenticationService,
    private projectService: ProjectService,
    private freelancerService: FreelancerService,
    private projectProposalService: ProjectProposalService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.user$.subscribe(user => {
      if (user && ('companyName' in user || 'hourlyRate' in user)) {
        this.user.set(user);
        this.fetchProjects(user);
        // console.log("User logged in:", user);
      } else if (!sessionStorage.getItem('accessToken')) {
        console.log("User redirect:", user);
        this.user.set(null);
        this.toastr.error('You must be logged in to access this page');
        this.router.navigate(['/auth']);
      }
    });
    this.createProjectForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      budget: [null, [Validators.required, Validators.min(1)]],
      duration: ['', [Validators.required, this.durationValidator]],
      requiredSkills: ['', [Validators.required]]
    });
    this.editProjectForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      budget: [null, [Validators.required, Validators.min(1)]],
      duration: ['', [Validators.required, this.durationValidator]],
      requiredSkills: ['', [Validators.required]]
    });

    // Subscribe to NgRx project error and show toast
    // this.subscriptions.add(
    //   this.store.select(selectProjectError).subscribe(error => {
    //     if (error) {
    //       this.toastr.error(error);
    //     }
    //   })
    // );
    // Subscribe to project list for updates (add/edit)
    this.subscriptions.add(
      this.store.select(selectAllProjects).subscribe(projects => {
        this.projects.set(projects);
      })
    );
    // Listen for addProjectSuccess to close modal and show toast
    this.subscriptions.add(
      this.actions$.pipe(ofType(ProjectActions.addProjectSuccess)).subscribe(() => {
        this.toastr.success('Project created successfully');
        this.closeCreateProjectModal();
        this.fetchProjects(this.user()!);
      })
    );
    // Listen for updateProjectSuccess to close modal and show toast
    this.subscriptions.add(
      this.actions$.pipe(ofType(ProjectActions.updateProjectSuccess)).subscribe(({ project }) => {
        this.toastr.success('Project updated successfully');
        this.selectedProject.set(project);
        this.closeEditProjectModal();
        this.fetchProjects(this.user()!);
      })
    );
    // Listen for deleteProjectSuccess to show toast, clear selection, and refresh projects
    this.subscriptions.add(
      this.actions$.pipe(ofType(ProjectActions.deleteProjectSuccess)).subscribe(() => {
        this.toastr.success('Project deleted successfully');
        this.clearSelectedProject();
        this.fetchProjects(this.user()!);
      })
    );
    // Listen for deleteProjectFailure to show error toast
    this.subscriptions.add(
      this.actions$.pipe(ofType(ProjectActions.deleteProjectFailure)).subscribe(({ error }) => {
        this.toastr.error(error || 'Failed to delete project');
      })
    );
  }

  durationValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;
    // Accepts '3d 4h 5m', '120', 'P2DT3H4M', etc.
    const regex = /^((\d+)d)?\s*((\d+)h)?\s*((\d+)m)?$|^P(\d+D)?(T(\d+H)?(\d+M)?)?$|^\d+$/;
    if (regex.test(value.trim())) {
      return null;
    }
    return { invalidDuration: true };
  }

  fetchProjects(auth: ClientModel | FreelancerModel, page: number = 1) {
    // console.log('Fetching projects for user:', auth);
    this.isLoading.set(true);
    if ('companyName' in auth) {
      this.projectService.getProjectsByClientId(auth.id, page, this.pageSize).subscribe({
        next: (res) => {
          if (res.success) {
            this.projects.set(res.data.data || []);
            this.pagination = res.data.pagination;
            this.totalPages.set(this.pagination?.totalPages || 1);
            this.currentPage.set(page);
          } else {
            this.toastr.error(res?.message || 'Failed to fetch projects');
          }
        },
        error: () => {
          this.toastr.error('Error fetching projects');
        },
        complete: () => this.isLoading.set(false)
      });
    } else if ('hourlyRate' in auth) {
      this.projectService.getAllProjects(page, this.pageSize, auth.username).subscribe({
        next: (res) => {
          if(res.success) {
            const filtered = res?.data?.data.filter((p: ProjectModel) => 
              p.freelancerId === auth.id && 
              (p.status === 'Completed' || p.status === 'In Progress' || p.status === 'Cancelled')
            );
            // console.log('Filtered projects:', filtered);
            this.projects.set(filtered || []);
            this.pagination = res.data.pagination;
            this.totalPages.set(this.pagination?.totalPages || 1);
            this.currentPage.set(page);
          } else {
            this.toastr.error(res?.message || 'Failed to fetch projects');
          }
        },
        error: () => {
          this.toastr.error('Error fetching projects');
        },
        complete: () => this.isLoading.set(false)
      });
    }
  }

  changePage(page: number) {
    if (!this.user()) return;
    this.fetchProjects(this.user()!, page);
  }

  selectProject(project: ProjectModel) {
    this.selectedProject.set(project);
    this.freelancerUsername.set(null);
    // console.log('Selected project:', project);
    if (project.freelancerId) {
      this.freelancerService.getFreelancerById(project.freelancerId).subscribe({
        next: (res) => {
          if (res.success && res.data) {
            // console.log('Freelancer data:', res.data);
            this.freelancerUsername.set(res.data.username);
          }
        },
        error: (error) => {
            // console.log('Freelancer data:', error);
          this.toastr.error('Error fetching freelancer details');
        }
      });
    }
  }

  clearSelectedProject() {
    this.selectedProject.set(null);
  }

  // Client actions
  createProject() {
    this.showCreateProjectModal.set(true);
  }
  closeCreateProjectModal() {
    this.showCreateProjectModal.set(false);
    this.createProjectForm.reset();
  }
  submitCreateProject() {
    if (this.createProjectForm.invalid || !this.user()) return;
    const form = this.createProjectForm.value;
    const newProject: CreateProjectModel = {
      title: form.title,
      description: form.description,
      budget: form.budget,
      duration: ConvertTimeSpan.toCSharpTimeSpan(form.duration),
      clientId: (this.user() as ClientModel).id,
      requiredSkills: form.requiredSkills.split(',').map((s: string) => ({ name: s.trim() }))
    };
    this.store.dispatch(ProjectActions.addProject({ project: newProject }));
  }
  editProject(project: ProjectModel) {
    const readableDuration = new TimespanToReadablePipe().transform(project.duration);
    this.editProjectForm.setValue({
      title: project.title,
      description: project.description,
      budget: project.budget,
      duration: readableDuration,
      requiredSkills: project.requiredSkills.map(s => s.name).join(', ')
    });
    this.showEditProjectModal.set(true);
  }
  closeEditProjectModal() {
    this.showEditProjectModal.set(false);
    if (this.editProjectForm) this.editProjectForm.reset();
  }
  submitEditProject() {
    if (this.editProjectForm.invalid || !this.selectedProject()) return;
    const form = this.editProjectForm.value;
    const update = {
      title: form.title,
      description: form.description,
      budget: form.budget,
      duration: ConvertTimeSpan.toCSharpTimeSpan(form.duration),
      requiredSkills: form.requiredSkills.split(',').map((s: string) => ({ name: s.trim() }))
    };
    this.store.dispatch(ProjectActions.updateProject({ projectId: this.selectedProject()!.id, project: update }));
  }
  deleteProject(project: ProjectModel) {
    if (!project?.id) return;
    if (!confirm('Are you sure you want to delete this project?')) return;
    this.store.dispatch(ProjectActions.deleteProject({ projectId: project.id }));
  }
  viewProposals(project: ProjectModel) {
    this.router.navigate(['/myproposal'], { queryParams: { search: project.title } });
  }

  // Search and filter
  onSearchChange(event: any) {
    this.searchTerm.set(event);
  }
  onStatusFilterChange(event: any) {
    this.statusFilter.set(event);
  }

  markAsComplete(projectId: string) {
    this.markAsCompleteLoading.set(true);
    this.projectProposalService.CompleteProject(projectId).subscribe({
      next: (res) => {
        if (res.success) {
          this.toastr.success('Project marked as completed!');
          this.fetchProjects(this.user()!);
        } else {
          this.toastr.error(res?.message || 'Failed to mark as complete');
        }
      },
      error: (err) => {
        this.toastr.error('Error marking as complete');
      },
      complete: () => {
        this.markAsCompleteLoading.set(false);
        this.clearSelectedProject();
      }
    });
  }

  cancelProject(projectId: string) {
    this.cancelProjectLoading.set(true);
    this.projectProposalService.CancelProject(projectId).subscribe({
      next: (res) => {
        if (res.success) {
          this.toastr.success('Project cancelled successfully!');
          this.fetchProjects(this.user()!);
        } else {
          this.toastr.error(res?.message || 'Failed to cancel project');
        }
      },
      error: (err) => {
        this.toastr.error('Error cancelling project');
      },
      complete: () => {
        this.cancelProjectLoading.set(false);
        this.clearSelectedProject();
      }
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
