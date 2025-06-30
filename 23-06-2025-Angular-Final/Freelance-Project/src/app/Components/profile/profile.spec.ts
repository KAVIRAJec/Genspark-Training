import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Profile } from './profile';
import { AuthenticationService } from '../../Services/auth.service';
import { ProjectService } from '../../Services/project.service';
import { ClientService } from '../../Services/client.service';
import { FreelancerService } from '../../Services/freelancer.service';
import { ImageUploadService } from '../../Services/ImageUpload.service';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';
import { ProjectModel } from '../../Models/Project.model';

function getMockClient(): ClientModel {
  return {
    id: '1', profileUrl: '', username: 'client', email: '', companyName: 'Test', location: '', isActive: true,
    createdAt: null, updatedAt: null, deletedAt: null, projects: []
  };
}
function getMockFreelancer(): FreelancerModel {
  return {
    id: '2', profileUrl: '', username: 'freelancer', email: '', experienceYears: 1, hourlyRate: 10, location: '', isActive: true,
    createdAt: null, updatedAt: null, deletedAt: null, skills: []
  };
}
function getMockProjects(): ProjectModel[] {
  return [
    { id: 'p1', title: 'Proj1', description: '', budget: 100, duration: '', isActive: true, status: 'Pending', createdAt: null, updatedAt: null, deletedAt: null, clientId: '1', freelancerId: null, requiredSkills: [], proposals: [] },
    { id: 'p2', title: 'Proj2', description: '', budget: 200, duration: '', isActive: true, status: 'Completed', createdAt: null, updatedAt: null, deletedAt: null, clientId: '1', freelancerId: null, requiredSkills: [], proposals: [] }
  ];
}

describe('Profile', () => {
  let component: Profile;
  let fixture: ComponentFixture<Profile>;
  let mockAuthService: any;
  let mockProjectService: any;
  let mockClientService: any;
  let mockFreelancerService: any;
  let mockImageUploadService: any;
  let mockToastr: any;
  let mockRouter: any;
  let mockRoute: any;

  beforeEach(async () => {
    mockAuthService = { user$: of(getMockClient()) };
    mockProjectService = {
      getProjectsByClientId: jasmine.createSpy().and.returnValue(of({ success: true, data: { data: getMockProjects() } })),
      getProjectsByFreelancerId: jasmine.createSpy().and.returnValue(of({ success: true, data: { data: getMockProjects() } }))
    };
    mockClientService = {};
    mockFreelancerService = { getFreelancerById: jasmine.createSpy().and.returnValue(of({ success: true, data: getMockFreelancer() })) };
    mockImageUploadService = {};
    mockToastr = jasmine.createSpyObj('ToastrService', ['success', 'error', 'info', 'warning']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockRoute = { queryParams: of({}), snapshot: { params: {} } };

    await TestBed.configureTestingModule({
      imports: [Profile],
      providers: [
        { provide: AuthenticationService, useValue: mockAuthService },
        { provide: ProjectService, useValue: mockProjectService },
        { provide: ClientService, useValue: mockClientService },
        { provide: FreelancerService, useValue: mockFreelancerService },
        { provide: ImageUploadService, useValue: mockImageUploadService },
        { provide: ToastrService, useValue: mockToastr },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set user and role as client and fetch projects', () => {
    component.ngOnInit();
    expect(component.user()?.id).toBe('1');
    expect(component.role()).toBe('client');
    expect(component.projects().length).toBeGreaterThan(0);
  });

  it('should set user and role as freelancer and fetch projects if viewing other freelancer', () => {
    mockRoute.queryParams = of({ freelancerId: '2' });
    mockFreelancerService.getFreelancerById.and.returnValue(of({ success: true, data: getMockFreelancer() }));
    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.ngOnInit();
    expect(component.user()?.id).toBe('2');
    expect(component.role()).toBe('freelancer');
    expect(component.projects().length).toBeGreaterThan(0);
  });

  it('should handle error if freelancer not found', () => {
    mockRoute.queryParams = of({ freelancerId: '404' });
    mockFreelancerService.getFreelancerById.and.returnValue(of({ success: false }));
    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.ngOnInit();
    expect(component.error()).toBe('Freelancer not found');
  });

  it('should handle error if project fetch fails', () => {
    mockProjectService.getProjectsByClientId.and.returnValue(throwError(() => new Error('fail')));
    component.fetchProjects('1', 'client');
    expect(component.error()).toBe('Failed to load projects');
  });
});
