import { Component, OnInit, signal, computed } from '@angular/core';
import { AuthenticationService } from '../../Services/auth.service';
import { ProjectService } from '../../Services/project.service';
import { ClientService } from '../../Services/client.service';
import { FreelancerService } from '../../Services/freelancer.service';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';
import { ProjectModel } from '../../Models/Project.model';
import { SkillModel } from '../../Models/Skill.model';
import { UpdateClientModel } from '../../Models/Client.model';
import { UpdateFreelancerModel } from '../../Models/Freelancer.model';
import { take } from 'rxjs';
import { NgIf, NgFor, TitleCasePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ImageUploadService } from '../../Services/ImageUpload.service';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-profile',
  imports: [NgIf, NgFor, FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {
  user = signal<ClientModel | FreelancerModel | null>(null);
  role = signal<'client' | 'freelancer' | null>(null);
  projects = signal<ProjectModel[]>([]);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  showEditModal = signal<boolean>(false);
  showDeactivateModal = signal<boolean>(false);
  showAddSkillModal = false;
  skillInput: string = '';
  pendingSkills: string[] = [];

  // For edit form
  editData: Partial<UpdateClientModel & UpdateFreelancerModel> = {};
  newSkills: string[] = [''];

  public isViewingOtherFreelancer = false;

  constructor(
    private authService: AuthenticationService,
    private projectService: ProjectService,
    private clientService: ClientService,
    private freelancerService: FreelancerService,
    private imageUploadService: ImageUploadService,
    private toast: ToastrService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const freelancerId = params['freelancerId'];
      this.isViewingOtherFreelancer = !!freelancerId;
      if (freelancerId) {
        this.freelancerService.getFreelancerById(freelancerId).subscribe(res => {
          if (res.success) {
            this.user.set(res.data);
            this.role.set('freelancer');
            this.fetchProjects(freelancerId, 'freelancer');
          } else {
            this.error.set('Freelancer not found');
          }
        });
      } else {
        this.authService.user$.subscribe(user => {
          if (user && 'companyName' in user) {
            this.user.set(user);
            this.role.set('client');
            this.fetchProjects(user.id, 'client');
          } else if (user && 'hourlyRate' in user) {
            this.user.set(user);
            this.role.set('freelancer');
            this.fetchProjects(user.id, 'freelancer');
          }
        });
      }
    });
  }

  fetchProjects(id: string, type: 'client' | 'freelancer') {
    this.loading.set(true);
    const obs = type === 'client'
      ? this.projectService.getProjectsByClientId(id, 1, 100)
      : this.projectService.getProjectsByFreelancerId(id, 1, 100);
    obs.pipe(take(1)).subscribe({
      next: (res) => {
        if (res.success) {
          this.projects.set(res.data.data);
        } else {
          this.error.set('Failed to load projects');
        }
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load projects');
        this.loading.set(false);
      }
    });
  }

  // Dashboard stats
  projectStats = computed(() => {
    const stats = { ongoing: 0, completed: 0, cancelled: 0, other: 0 };
    for (const p of this.projects()) {
      if (p.status === 'Pending' || 'In Progress') stats.ongoing++;
      else if (p.status === 'Completed' ) stats.completed++;
      else if (p.status === 'Cancelled') stats.cancelled++;
      else stats.other++;
    }
    return stats;
  });

  // Edit profile
  openEditModal() {
    if (!this.user()) return;
    this.editData = { ...this.user() };
    this.showEditModal.set(true);
  }
  saveEdit() {
    if (!this.user()) return;
    const id = this.user()!.id;
    if (this.role() === 'client') {
      this.clientService.updateClient(id, this.editData as UpdateClientModel).pipe(take(1)).subscribe({
        next: () => {
          this.showEditModal.set(false);
          this.authService.getMe().subscribe();
          this.toast.success('Profile updated successfully!');
        },
        error: () => this.toast.error('Failed to update profile!')
      });
    } else if (this.role() === 'freelancer') {
      this.freelancerService.updateFreelancer(id, this.editData as UpdateFreelancerModel).pipe(take(1)).subscribe({
        next: () => {
          this.showEditModal.set(false);
          this.authService.getMe().subscribe();
          this.toast.success('Profile updated successfully!');
        },
        error: () => this.toast.error('Failed to update profile!')
      });
    }
  }

  // Deactivate account
  openDeactivateModal() {
    this.showDeactivateModal.set(true);
  }
  confirmDeactivate() {
    if (!this.user()) return;
    const id = this.user()!.id;
    if (this.role() === 'client') {
      this.clientService.deleteClient(id).pipe(take(1)).subscribe({
        next: () => {
          this.authService.logout().subscribe();
          this.toast.success('Account deactivated!');
          this.router.navigate(['/']); 

        },
        error: () => this.toast.error('Failed to deactivate account!')
      });
    } else if (this.role() === 'freelancer') {
      this.freelancerService.deleteFreelancer(id).pipe(take(1)).subscribe({
        next: () => {
          this.authService.logout().subscribe();
          this.toast.success('Account deactivated!');
          this.router.navigate(['/']);
        },
        error: () => this.toast.error('Failed to deactivate account!')
      });
    }
  }
  cancelDeactivate() {
    this.showDeactivateModal.set(false);
  }

  get freelancerSkills(): SkillModel[] {
    const user = this.user();
    return user && 'skills' in user ? user.skills : [];
  }

  onProfileImageChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || !input.files[0] || !this.user()) return;
    const file = input.files[0];
    const id = this.user()!.id;
    const role = this.role();
    if (!role) return;
    this.imageUploadService.UploadImage(file).subscribe({
      next: (res) => {
        if (res.success) {
          const imageUrl = typeof res.data === 'string' ? res.data : res.data.imageUrl;
          if(role === 'client') {
            this.clientService.updateClient(id, { profileUrl: imageUrl } as UpdateClientModel).pipe(take(1)).subscribe({
              next: () => {
                this.authService.getMe().subscribe();
                this.toast.success('Profile image updated!');
              },
              error: () => this.toast.error('Failed to update profile image!')
            });
          }
          else if(role === 'freelancer') {
            this.freelancerService.updateFreelancer(id, { profileUrl: imageUrl } as UpdateFreelancerModel).pipe(take(1)).subscribe({
              next: () => {
                this.authService.getMe().subscribe();
                this.toast.success('Profile image updated!');
              },
              error: () => this.toast.error('Failed to update profile image!')
            });
          }
        } else {
          this.toast.error('Failed to upload image!');
        }
      },
      error: () => this.toast.error('Failed to upload image!')
    });
  }

  logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.toast.success('Logged out!');
        this.router.navigate(['/']);
      },
      error: () => this.toast.error('Logout failed!')
    });
  }

  // Add skills 
  openAddSkillModal() {
    this.skillInput = '';
    this.pendingSkills = [];
    this.showAddSkillModal = true;
  }
  closeAddSkillModal() {
    this.showAddSkillModal = false;
  }
  onSkillInputKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter' && this.skillInput.trim()) {
      event.preventDefault();
      this.pendingSkills.push(this.skillInput.trim());
      this.skillInput = '';
    }
  }
  removePendingSkill(index: number) {
    this.pendingSkills.splice(index, 1);
  }
  saveSkills() {
    if (!this.user() || this.role() !== 'freelancer') return;
    const user = this.user() as FreelancerModel;
    const validSkills = this.pendingSkills.filter(s => s.trim());
    if (!validSkills.length) return;
    const newSkillsArr = [...(user.skills || []), ...validSkills.map(name => ({ name: name }))];
    this.freelancerService.updateFreelancer(user.id, { skills: newSkillsArr } as UpdateFreelancerModel)
      .pipe(take(1)).subscribe({
      next: () => {
        this.authService.getMe().subscribe();
        this.toast.success('Skills added successfully!');
        this.closeAddSkillModal();
      },
      error: () => {
        this.toast.error('Failed to add skills!');
      }
      });
  }
}
