import { TestBed } from '@angular/core/testing';
import { ProposalService } from '../proposal.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('ProposalService', () => {
  let service: ProposalService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['get', 'post', 'put', 'delete']);
    TestBed.configureTestingModule({
      providers: [
        ProposalService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(ProposalService);
  });

  it('should create proposal', () => {
    httpSpy.post.and.returnValue(of({}));
    service.CreateProposal({} as any).subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should get proposal by id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.GetProposalById('id').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get proposals by freelancer id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.GetProposalsByFreelancerId('id').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get proposals by client id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.GetProposalsByClientId('id').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get all proposals', () => {
    httpSpy.get.and.returnValue(of({}));
    service.GetAllProposals().subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should update proposal', () => {
    httpSpy.put.and.returnValue(of({}));
    service.UpdateProposal('id', {} as any).subscribe();
    expect(httpSpy.put).toHaveBeenCalled();
  });

  it('should delete proposal', () => {
    httpSpy.delete.and.returnValue(of({}));
    service.DeleteProposal('id').subscribe();
    expect(httpSpy.delete).toHaveBeenCalled();
  });
});
