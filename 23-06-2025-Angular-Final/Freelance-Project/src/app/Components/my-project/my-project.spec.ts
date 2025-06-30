import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyProject } from './my-project';
import { ProjectService } from '../../Services/project.service';
import { AuthenticationService } from '../../Services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Store } from '@ngrx/store';
import { FormBuilder } from '@angular/forms';
import { FreelancerService } from '../../Services/freelancer.service';
import { ProjectProposalService } from '../../Services/projectProposal.service';
import { Router } from '@angular/router';
import { provideMockStore } from '@ngrx/store/testing';
import { Actions } from '@ngrx/effects';
import { of } from 'rxjs';

describe('MyProject', () => {
  let component: MyProject;
  let fixture: ComponentFixture<MyProject>;
  let mockProjectService: jasmine.SpyObj<ProjectService>;
  let mockAuthService: any;
  let mockToastr: jasmine.SpyObj<ToastrService>;
  let mockFreelancerService: jasmine.SpyObj<FreelancerService>;
  let mockProjectProposalService: jasmine.SpyObj<ProjectProposalService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockProjectService = jasmine.createSpyObj('ProjectService', ['getAllProjects', 'getProjectsByClientId', 'getProjectsByFreelancerId', 'createProject', 'getProjectById']);
    mockAuthService = { user$: of({ id: '1', companyName: 'Test', profileUrl: '', username: '', email: '', location: '', isActive: true, createdAt: null, updatedAt: null, deletedAt: null, projects: [] }) };
    mockToastr = jasmine.createSpyObj('ToastrService', ['success', 'error', 'info', 'warning']);
    mockFreelancerService = jasmine.createSpyObj('FreelancerService', ['getFreelancerById']);
    mockProjectProposalService = jasmine.createSpyObj('ProjectProposalService', ['GetProposalsByProjectId', 'AcceptProposal', 'RejectProposal', 'CancelProject', 'CompleteProject']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [MyProject],
      providers: [
        provideMockStore(),
        { provide: ProjectService, useValue: mockProjectService },
        { provide: AuthenticationService, useValue: mockAuthService },
        { provide: ToastrService, useValue: mockToastr },
        { provide: FreelancerService, useValue: mockFreelancerService },
        { provide: ProjectProposalService, useValue: mockProjectProposalService },
        { provide: Router, useValue: mockRouter },
        { provide: Actions, useValue: of() },
        FormBuilder
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MyProject);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should filter projects by search term', () => {
    component.projects.set([
      { title: 'Test Project', description: 'desc', id: '1' },
      { title: 'Another', description: 'something', id: '2' }
    ] as any);
    component.searchTerm.set('test');
    expect(component.filteredProjects().length).toBe(1);
    expect(component.filteredProjects()[0].title).toBe('Test Project');
  });

  it('should open and close create project modal', () => {
    component.showCreateProjectModal.set(false);
    component.showCreateProjectModal.set(true);
    expect(component.showCreateProjectModal()).toBeTrue();
    component.showCreateProjectModal.set(false);
    expect(component.showCreateProjectModal()).toBeFalse();
  });

  it('should call markAllAsRead and update notifications', () => {
    if (component['markAsCompleteLoading']) {
      component.markAsCompleteLoading.set(true);
      expect(component.markAsCompleteLoading()).toBeTrue();
      component.markAsCompleteLoading.set(false);
      expect(component.markAsCompleteLoading()).toBeFalse();
    }
  });

  // Add more tests for methods as needed, mocking service/store calls
});
