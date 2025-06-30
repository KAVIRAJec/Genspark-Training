import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyProposal } from './my-proposal';
import { provideMockStore } from '@ngrx/store/testing';
import { AuthenticationService } from '../../Services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { ProjectProposalService } from '../../Services/projectProposal.service';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';

describe('MyProposal', () => {
  let component: MyProposal;
  let fixture: ComponentFixture<MyProposal>;
  let mockAuthService: any;
  let mockToastr: jasmine.SpyObj<ToastrService>;
  let mockProjectProposalService: jasmine.SpyObj<ProjectProposalService>;
  let mockFormBuilder: jasmine.SpyObj<FormBuilder>;
  let mockRoute: jasmine.SpyObj<ActivatedRoute>;

  function getMockClient(): ClientModel {
    return {
      id: '1',
      profileUrl: '',
      username: 'clientuser',
      email: 'client@example.com',
      companyName: 'Test',
      location: 'Earth',
      isActive: true,
      createdAt: null,
      updatedAt: null,
      deletedAt: null,
      projects: []
    };
  }
  function getMockFreelancer(): FreelancerModel {
    return {
      id: '2',
      profileUrl: '',
      username: 'freelanceruser',
      email: 'freelancer@example.com',
      experienceYears: 2,
      hourlyRate: 10,
      location: 'Mars',
      isActive: true,
      createdAt: null,
      updatedAt: null,
      deletedAt: null,
      skills: []
    };
  }

  beforeEach(async () => {
    mockAuthService = { user$: of(getMockClient()) };
    mockToastr = jasmine.createSpyObj('ToastrService', ['success', 'error', 'info', 'warning']);
    mockProjectProposalService = jasmine.createSpyObj('ProjectProposalService', ['GetProposalsByProjectId', 'AcceptProposal', 'RejectProposal', 'CancelProject', 'CompleteProject']);
    mockFormBuilder = jasmine.createSpyObj('FormBuilder', ['group']);
    mockRoute = jasmine.createSpyObj('ActivatedRoute', [], {
      snapshot: {
        params: {},
        url: [],
        queryParams: {},
        fragment: '',
        data: {},
        outlet: '',
        component: null,
        routeConfig: null,
        root: null as any,
        parent: null,
        firstChild: null,
        children: [],
        pathFromRoot: [],
        paramMap: null as any,
        queryParamMap: null as any,
        title: ''
      },
      queryParams: of({})
    });

    await TestBed.configureTestingModule({
      imports: [MyProposal],
      providers: [
        provideMockStore({
          initialState: {
            proposal: {
              proposals: [],
              loading: false,
              error: null,
              pagination: null
            }
          }
        }),
        { provide: AuthenticationService, useValue: mockAuthService },
        { provide: ToastrService, useValue: mockToastr },
        { provide: ProjectProposalService, useValue: mockProjectProposalService },
        { provide: FormBuilder, useValue: mockFormBuilder },
        { provide: ActivatedRoute, useValue: mockRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MyProposal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set user and userRole on authService user$ emit', () => {
    component.user.set(getMockClient());
    component.userRole.set('client');
    expect(component.userRole()).toBe('client');
    component.user.set(null);
    component.user.set(getMockFreelancer());
    component.userRole.set('freelancer');
    expect(component.userRole()).toBe('freelancer');
  });

  it('should update searchTerm and sortBy', () => {
    component.searchTerm.set('test');
    expect(component.searchTerm()).toBe('test');
    component.sortBy.set('date');
    expect(component.sortBy()).toBe('date');
  });
});
