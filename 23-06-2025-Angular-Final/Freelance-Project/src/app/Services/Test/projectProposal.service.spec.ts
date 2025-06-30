import { TestBed } from '@angular/core/testing';
import { ProjectProposalService } from '../projectProposal.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('ProjectProposalService', () => {
  let service: ProjectProposalService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    TestBed.configureTestingModule({
      providers: [
        ProjectProposalService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(ProjectProposalService);
  });

  it('should get proposals by project id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.GetProposalsByProjectId('project').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should accept proposal', () => {
    httpSpy.post.and.returnValue(of({}));
    service.AcceptProposal('project', 'proposal').subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should reject proposal', () => {
    httpSpy.post.and.returnValue(of({}));
    service.RejectProposal('project', 'proposal').subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should cancel project', () => {
    httpSpy.post.and.returnValue(of({}));
    service.CancelProject('project').subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should complete project', () => {
    httpSpy.post.and.returnValue(of({}));
    service.CompleteProject('project').subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });
});
