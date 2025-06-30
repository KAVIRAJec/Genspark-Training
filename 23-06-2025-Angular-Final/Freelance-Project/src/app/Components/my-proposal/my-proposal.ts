import { Component, computed, effect, signal } from '@angular/core';
import { Store } from '@ngrx/store';
import { AuthenticationService } from '../../Services/auth.service';
import { ProposalModel } from '../../Models/Proposal.model';
import { selectAllProposals, selectProposalLoading, selectProposalError, selectProposalPagination } from '../../NgRx/Proposal/proposal.selector';
import * as ProposalActions from '../../NgRx/Proposal/proposal.actions';
import { ToastrService } from 'ngx-toastr';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';
import { CommonModule } from '@angular/common';
import { ProjectProposalService } from '../../Services/projectProposal.service';
import { TimespanToReadablePipe } from '../../Pipes/timespan-to-readable.pipe';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { PaginationModel } from '../../Models/PaginationModel';
import { DurationFormatPipe } from '../../Pipes/duration-format.pipe';
import { ConvertTimeSpan } from '../../Misc/ConvertTimeSpan';

@Component({
  selector: 'app-my-proposal',
  templateUrl: './my-proposal.html',
  styleUrl: './my-proposal.css',
  standalone: true,
  imports: [CommonModule, FormsModule, DurationFormatPipe, TimespanToReadablePipe, ReactiveFormsModule]
})
export class MyProposal {
  proposals;
  loading;
  error;
  user = signal<ClientModel | FreelancerModel | null>(null);
  userRole = signal<'client' | 'freelancer' | null>(null);

  editForm: FormGroup;

  pagination!: ReturnType<typeof this.store.selectSignal>;
  page = signal<number>(1);
  pageSize = signal<number>(5);
  searchTerm = signal<string>('');
  selectedProjectTitle = signal<string>('');
  sortBy = signal<string>('');

  constructor(
    private store: Store,
    private authService: AuthenticationService,
    private toastr: ToastrService,
    private projectProposalService: ProjectProposalService,
    private fb: FormBuilder,
    private route: ActivatedRoute
  ) {
    this.proposals = this.store.selectSignal(selectAllProposals);
    this.loading = this.store.selectSignal(selectProposalLoading);
    this.error = this.store.selectSignal(selectProposalError);
    this.pagination = this.store.selectSignal(selectProposalPagination);
    this.authService.user$.subscribe(user => {
      this.user.set(user);
      if (user) {
        if ('companyName' in user) {
          this.userRole.set('client');
        } else {
          this.userRole.set('freelancer');
        }
      }
    });
    effect(() => {
      const user = this.user();
      if (!user) return;
      const role = this.userRole();
      const page = this.page();
      const pageSize = this.pageSize();
      const search = this.searchTerm();
      if (role === 'freelancer') {
        this.store.dispatch(ProposalActions.loadProposals({
          freelancerId: user.id,
          page,
          pageSize,
          search
        }));
      } else if (role === 'client') {
        this.store.dispatch(ProposalActions.loadProposals({
          clientId: user.id,
          page,
          pageSize,
          search
        }));
      }
    });
    effect(() => {
      if (this.error()) {
        this.toastr.error(typeof this.error() === 'string' ? this.error() ?? 'An error occurred' : 'An error occurred');
      }
    });
    this.editForm = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      proposedAmount: [null, [Validators.required, Validators.min(1)]],
      proposedDuration: ['', [Validators.required, this.durationValidator]]
    });
    this.route.queryParams.subscribe(params => {
      if (params['search']) {
        this.searchTerm.set(params['search']);
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

  // For edit modal
  editingProposal = signal<ProposalModel | null>(null);
  showEditModal = signal(false);

  canEditOrWithdraw(proposal: ProposalModel) {
    return this.userRole() === 'freelancer' && proposal.freelancer.id === this.user()?.id && !proposal.isAccepted && proposal.isActive && !proposal.isRejected;
  }

  canAcceptOrReject(proposal: ProposalModel) {
    return this.userRole() === 'client' && proposal.project.clientId === this.user()?.id && !proposal.isAccepted && proposal.isActive && !proposal.isRejected;
  }

  onEdit(proposal: ProposalModel) {
    const readableDuration = new TimespanToReadablePipe().transform(proposal.proposedDuration);
    this.editingProposal.set(proposal);
    this.editForm.setValue({
      description: proposal.description,
      proposedAmount: proposal.proposedAmount,
      proposedDuration: readableDuration
    });
    this.showEditModal.set(true);
  }

  onWithdraw(proposal: ProposalModel) {
    this.store.dispatch(ProposalActions.deleteProposal({ proposalId: proposal.id }));
    this.toastr.success('Proposal withdrawn');
  }

  onAccept(proposal: ProposalModel) {
    const projectId = proposal.project.id;
    const proposalId = proposal.id;
    this.projectProposalService.AcceptProposal(projectId, proposalId).subscribe({
      next: (res) => {
        if (res.success) {
          this.toastr.success('Proposal accepted!');
          this.store.dispatch(ProposalActions.loadProposals({}));
        } else {
          this.toastr.error(res?.message || 'Failed to accept proposal');
        }
      },
      error: () => {
        this.toastr.error('Error accepting proposal');
      }
    });
  }

  onReject(proposal: ProposalModel) {
    const projectId = proposal.project.id;
    const proposalId = proposal.id;
    this.projectProposalService.RejectProposal(projectId, proposalId).subscribe({
      next: (res) => {
        if (res.success) {
          this.toastr.success('Proposal rejected!');
          this.store.dispatch(ProposalActions.loadProposals({}));
        } else {
          this.toastr.error(res?.message || 'Failed to reject proposal');
        }
      },
      error: () => {
        this.toastr.error('Error rejecting proposal');
      }
    });
  }

  // For edit modal save
  onSaveEdit() {
    if (!this.editingProposal() || this.editForm.invalid) return;
    this.editForm.value.proposedDuration = ConvertTimeSpan.toCSharpTimeSpan(this.editForm.value.proposedDuration);
    this.store.dispatch(ProposalActions.updateProposal({
      proposalId: this.editingProposal()!.id,
      proposal: this.editForm.value
    }));
    this.toastr.success('Proposal updated');
    this.showEditModal.set(false);
  }

  onSearchChange(value: string) {
    this.searchTerm.set(value);
    this.page.set(1);
  }
  onProjectFilterChange(value: string) {
    this.selectedProjectTitle.set(value);
    this.page.set(1);
  }
  onPageChange(newPage: number) {
    this.page.set(newPage);
  }
  onPageSizeChange(newSize: number) {
    this.pageSize.set(newSize);
    this.page.set(1);
  }
  onSortByChange(value: string) {
    this.sortBy.set(value);
    this.page.set(1);
  }
  get pageSizeValue() {
    return this.pageSize();
  }
  setPageSize(val: number) {
    this.onPageSizeChange(val);
  }

  projectTitles = computed(() => {
    const user = this.user();
    if (user && this.userRole() === 'client' && 'projects' in user) {
      return user.projects.map((p: any) => p.title);
    }
    if (user && this.userRole() === 'freelancer') {
      return this.proposals().map((p: any) => p.project.title);
    }
    return [];
  });
  get pageNumbers() {
    const pag = this.pagination() as PaginationModel | null;
    if (!pag || !pag.totalPages) return [];
    return Array.from({ length: pag.totalPages }, (_, i) => i + 1);
  }
  get paginationModel(): PaginationModel | null {
    return this.pagination() as PaginationModel | null;
  }

  // Client-side sorting in filteredProposals
  filteredProposals = computed(() => {
    let proposals = this.proposals() || [];
    const selectedTitle = this.selectedProjectTitle();
    const sortBy = this.sortBy();

    // Filter by project title if selected
    if (selectedTitle) {
      proposals = proposals.filter((p: any) => p.project.title === selectedTitle);
    }

    // Sort by status
    if (sortBy === 'Pending') {
      proposals = [...proposals].sort((a, b) => {
        const aPending = !a.isAccepted && !a.isRejected;
        const bPending = !b.isAccepted && !b.isRejected;
        return (bPending ? 1 : 0) - (aPending ? 1 : 0);
      });
    } else if (sortBy === 'Accepted') {
      proposals = [...proposals].sort((a, b) => {
        return (b.isAccepted ? 1 : 0) - (a.isAccepted ? 1 : 0);
      });
    } else if (sortBy === 'Rejected') {
      proposals = [...proposals].sort((a, b) => {
        return (b.isRejected ? 1 : 0) - (a.isRejected ? 1 : 0);
      });
    }
    return proposals;
  });
}
