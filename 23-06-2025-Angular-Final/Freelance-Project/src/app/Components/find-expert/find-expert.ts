import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import { selectAllFreelancers, selectFreelancerPagination, selectFreelancerLoading, selectFreelancerError } from '../../NgRx/Freelancer/freelancer.selector';
import * as FreelancerActions from '../../NgRx/Freelancer/freelancer.actions';
import { Router, ActivatedRoute } from '@angular/router';
import { AsyncPipe, NgFor, NgIf, NgStyle } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-find-expert',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule, AsyncPipe, NgStyle],
  templateUrl: './find-expert.html',
  styleUrl: './find-expert.css'
})
export class FindExpert implements OnInit, OnDestroy {
  freelancers$ :any;
  pagination: any = null;
  skills: string[] = ['Web Development', 'App Development', 'AI/ML', 'UI/UX', 'SEO'];
  locations: string[] = ['Asia', 'Europe', 'North America', 'Australia'];
  search = '';
  selectedSkill = '';
  selectedLocation = '';
  readonly pageSize = 12;

  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  loading$: any;
  error$: any;

  constructor(private store: Store, private router: Router, private toastr: ToastrService, private route: ActivatedRoute) {
    this.loading$ = this.store.select(selectFreelancerLoading);
    this.error$ = this.store.select(selectFreelancerError);
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['search']) {
        this.search = params['search'];
        this.searchSubject.next(this.search);
      }
    });
    this.freelancers$ = this.store.select(selectAllFreelancers);
    this.store.select(selectFreelancerPagination).subscribe(pagination => {
      this.pagination = pagination;
    });
    this.freelancers$.subscribe((freelancers: any[]) => {
      if (freelancers && freelancers.length) {
        // Extract unique skills and locations from the loaded freelancers
        const skillSet = new Set<string>();
        const locationSet = new Set<string>();
        freelancers.forEach((f: any) => {
          if (Array.isArray(f.skills)) {
            (f.skills as (any)[]).forEach((s: any) => 
              skillSet.add(s.name)
            );
          }
          if (f.location) {
            locationSet.add(f.location);
          }
        });
        this.skills = Array.from(skillSet).sort();
        this.locations = Array.from(locationSet).sort();
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
    } else if (this.selectedLocation) {
      search = this.selectedLocation;
    }
    this.store.dispatch(FreelancerActions.loadFreelancers({
      page,
      pageSize: this.pageSize,
      search: search
    }));
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  viewDetails(freelancer: any) {
    if(sessionStorage.getItem('accessToken')) {
      this.router.navigate(['/profile'], { queryParams: { freelancerId: freelancer.id } });
    } else {
      this.toastr.info('Please log in to view freelancer details.', 'Info');
    }
  }
}