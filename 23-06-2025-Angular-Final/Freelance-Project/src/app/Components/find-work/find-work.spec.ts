import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FindWork } from './find-work';
import { Store } from '@ngrx/store';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../../Services/auth.service';
import { FormBuilder } from '@angular/forms';
import { Actions } from '@ngrx/effects';
import { of, Subject } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('FindWork', () => {
  let component: FindWork;
  let fixture: ComponentFixture<FindWork>;
  let storeSpy: jasmine.SpyObj<Store<any>>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let authServiceSpy: jasmine.SpyObj<AuthenticationService>;
  let fb: FormBuilder;
  let actionsSpy: jasmine.SpyObj<Actions>;
  let routeSpy: any;
  let queryParamsSubject: Subject<any>;

  beforeEach(async () => {
    storeSpy = jasmine.createSpyObj('Store', ['select', 'dispatch']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error', 'info']);
    authServiceSpy = jasmine.createSpyObj('AuthenticationService', [], { user$: of(null) });
    fb = new FormBuilder();
    actionsSpy = jasmine.createSpyObj('Actions', ['pipe']);
    queryParamsSubject = new Subject();
    routeSpy = { queryParams: queryParamsSubject.asObservable() };
    storeSpy.select.and.returnValue(of([]));
    actionsSpy.pipe.and.returnValue(of({ type: 'addProposalSuccess' }));

    await TestBed.configureTestingModule({
      imports: [FindWork],
      providers: [
        { provide: Store, useValue: storeSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: AuthenticationService, useValue: authServiceSpy },
        { provide: FormBuilder, useValue: fb },
        { provide: Actions, useValue: actionsSpy },
        { provide: ActivatedRoute, useValue: routeSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(FindWork);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should update search and trigger searchSubject on query param', () => {
    const nextSpy = spyOn((component as any).searchSubject, 'next');
    queryParamsSubject.next({ search: 'Angular' });
    expect(component.search).toBe('Angular');
    expect(nextSpy).toHaveBeenCalledWith('Angular');
  });

  it('should call loadPage and dispatch action on onFilter', () => {
    const loadPageSpy = spyOn(component, 'loadPage');
    component.onFilter();
    expect(loadPageSpy).toHaveBeenCalledWith(1, component.search);
  });

  it('should dispatch ProjectActions.loadProjects on loadPage', () => {
    component.selectedSkill = 'Angular';
    component.loadPage(2);
    expect(storeSpy.dispatch).toHaveBeenCalled();
  });

  it('should open proposal modal if freelancers$ is set', () => {
    component.freelancers$ = { id: 'f1' };
    const openSpy = spyOn(component, 'openProposalModal');
    component.submitProposalButton({ id: 'p1' });
    expect(openSpy).toHaveBeenCalledWith({ id: 'p1' });
  });

  it('should show info toast if freelancers$ is not set', () => {
    component.freelancers$ = null;
    component.submitProposalButton({ id: 'p1' });
    expect(toastrSpy.info).toHaveBeenCalled();
  });
});
