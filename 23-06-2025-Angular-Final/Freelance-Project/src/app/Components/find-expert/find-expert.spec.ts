import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FindExpert } from './find-expert';
import { Store } from '@ngrx/store';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('FindExpert', () => {
  let component: FindExpert;
  let fixture: ComponentFixture<FindExpert>;
  let storeSpy: jasmine.SpyObj<Store<any>>;
  let routerSpy: jasmine.SpyObj<Router>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let routeSpy: any;
  let queryParamsSubject: Subject<any>;

  beforeEach(async () => {
    storeSpy = jasmine.createSpyObj('Store', ['select', 'dispatch']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error', 'info']);
    queryParamsSubject = new Subject();
    routeSpy = { queryParams: queryParamsSubject.asObservable() };
    storeSpy.select.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [FindExpert],
      providers: [
        { provide: Store, useValue: storeSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: ActivatedRoute, useValue: routeSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(FindExpert);
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
});
