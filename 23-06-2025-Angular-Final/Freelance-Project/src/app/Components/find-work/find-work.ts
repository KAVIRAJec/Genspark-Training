import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import { selectAllProjects, selectProjectError, selectProjectLoading, selectProjectPagination } from '../../NgRx/Project/project.selector';
import * as ProjectActions from '../../NgRx/Project/project.actions';
import { Router, ActivatedRoute } from '@angular/router';
import { AbstractControl, FormsModule, ReactiveFormsModule, ValidationErrors } from '@angular/forms';
import { AsyncPipe, DatePipe, NgFor, NgIf, NgStyle } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { TimespanToReadablePipe } from '../../Pipes/timespan-to-readable.pipe';
import { AuthenticationService } from '../../Services/auth.service';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { selectProposalLoading, selectProposalError } from '../../NgRx/Proposal/proposal.selector';
import * as ProposalActions from '../../NgRx/Proposal/proposal.actions';
import { Actions, ofType } from '@ngrx/effects';
import { DurationFormatPipe } from '../../Pipes/duration-format.pipe';
import { CreateProposalModel } from '../../Models/Proposal.model';
import { ConvertTimeSpan } from '../../Misc/ConvertTimeSpan';

@Component({
  selector: 'app-find-work',
  imports: [FormsModule, DatePipe, DurationFormatPipe, AsyncPipe, NgIf, NgFor, NgStyle, TimespanToReadablePipe, ReactiveFormsModule],
  templateUrl: './find-work.html',
  styleUrl: './find-work.css'
})
export class FindWork implements OnInit, OnDestroy {
  projects$: any;
  freelancers$: any;
  pagination: any = null;
  skills: string[] = [];
  search = '';
  selectedSkill = '';
  readonly pageSize = 12;

  loading$: any;
  error$: any;
  proposalLoading$;
  proposalError$;
  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  // Proposal modal states
  showProposalModal = false;
  proposalForm!: FormGroup;
  submittingProposal = false;
  selectedProject: any = null;
  currentUserId: string = '';
  private proposalSuccessSub?: any;

  constructor(
    private store: Store,
    private router: Router,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private authService: AuthenticationService,
    private fb: FormBuilder,
    private actions$: Actions
  ) {
    this.loading$ = this.store.select(selectProjectLoading);
    this.error$ = this.store.select(selectProjectError);
    this.proposalLoading$ = this.store.select(selectProposalLoading);
    this.proposalError$ = this.store.select(selectProposalError);
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['search']) {
        this.search = params['search'];
        this.searchSubject.next(this.search);
      }
    });
    this.projects$ = this.store.select(selectAllProjects);
    this.store.select(selectProjectPagination).subscribe(pagination => {
      this.pagination = pagination;
    });
    this.projects$.subscribe((projects: any[]) => {
      if (projects && projects.length) {
        // Extract unique skills from loaded projects
        const skillSet = new Set<string>();
        projects.forEach((p: any) => {
          if (Array.isArray(p.requiredSkills)) {
            (p.requiredSkills as (any)[]).forEach((s: any) => skillSet.add(s.name));
          }
        });
        this.skills = Array.from(skillSet).sort();
      }
    });
    this.error$.subscribe((error: any) => {
      if (error) {
        let errorMessage = 'Login failed. Please try again.';
          if (error?.errors && typeof error.errors === 'object') {
            const firstKey = Object.keys(error.errors)[0];
            if (error && typeof error === 'object' && 'message' in error) {
              this.toastr.error((error as any).message, 'Error');
            } else if (firstKey && Array.isArray(error.errors[firstKey]) && error.errors[firstKey].length > 0) {
              errorMessage = error.errors[firstKey][0];
              this.toastr.error(errorMessage, 'Error');
            } 
          } else if (typeof error === 'string') {
              this.toastr.error(error, 'Error');
          }
      }
    });
    this.searchSubject.pipe(
      debounceTime(3000),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe((searchValue) => {
      this.loadPage(1, searchValue);
    });
    this.loadPage(1);

    this.authService.user$.subscribe((user: ClientModel | FreelancerModel | null) => {
        if (user && 'hourlyRate' in user) {
          this.freelancers$ = user;
        } else {
          this.freelancers$ = null;
        }
    });

    this.proposalForm = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      proposedAmount: [null, [Validators.required, Validators.min(1)]],
      proposedDuration: ['', [Validators.required, this.durationValidator]]
    });
    // Get current user id from authService
    this.authService.user$.subscribe(user => {
      if (user && 'id' in user) {
        this.currentUserId = user.id;
      }
    });
    // Listen for proposal add success/failure
    this.proposalSuccessSub = this.actions$.pipe(
      ofType(ProposalActions.addProposalSuccess, ProposalActions.addProposalFailure)
    ).subscribe(action => {
      this.submittingProposal = false;
      if (action.type === ProposalActions.addProposalSuccess.type) {
        this.toastr.success('Proposal submitted successfully!');
        this.closeProposalModal();
      } else {
        this.toastr.error((action as any).error.message || (action as any).error || 'Failed to submit proposal');
      }
    });
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

  openProposalModal(project: any) {
    this.selectedProject = project;
    this.showProposalModal = true;
    this.proposalForm.reset();
  }

  closeProposalModal() {
    this.showProposalModal = false;
    this.selectedProject = null;
  }

  submitProposal() {
    if (this.proposalForm.invalid || !this.selectedProject) return;
    this.submittingProposal = true;
    const form = this.proposalForm.value;
    console.log('Submitting proposal form:', form);
    const newProposal: CreateProposalModel = {
      description: form.description,
      proposedAmount: form.proposedAmount,
      proposedDuration: ConvertTimeSpan.toCSharpTimeSpan(form.proposedDuration),
      freelancerId: this.currentUserId,
      projectId: this.selectedProject.id
    };
    console.log('Submitting proposal:', newProposal);
    this.store.dispatch(ProposalActions.addProposal({ proposal: newProposal }));
  }

  changePage(page: number): void {
    this.pagination.page = page;
    this.onFilter();
  }

  onSearchChange(value: string) {
    this.searchSubject.next(value);
  }

  onFilter() {
    this.loadPage(1, this.search);
  }

  loadPage(page: number, searchOverride?: string) {
    let search = typeof searchOverride === 'string' ? searchOverride : this.search;
    if (this.selectedSkill) {
      search = this.selectedSkill;
    }
    this.store.dispatch(ProjectActions.loadProjects({
      page,
      pageSize: this.pageSize,
      search: search,
      sortBy: 'Pending'
    }));
  }

  submitProposalButton(project: any) {
    if(this.freelancers$) {
      this.openProposalModal(project);
    } else {
      this.toastr.info('Please log in as freelancer to submit a proposal.', 'Info');
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.proposalSuccessSub) this.proposalSuccessSub.unsubscribe();
  }
}
