import { TestBed } from '@angular/core/testing';
import { ProjectService } from '../project.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('ProjectService', () => {
  let service: ProjectService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['get', 'post', 'put', 'delete']);
    TestBed.configureTestingModule({
      providers: [
        ProjectService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(ProjectService);
  });

  it('should get all projects', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getAllProjects().subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get project by id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getProjectById('id').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should create project', () => {
    httpSpy.post.and.returnValue(of({}));
    service.createProject({} as any).subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should update project', () => {
    httpSpy.put.and.returnValue(of({}));
    service.updateProject('id', {} as any).subscribe();
    expect(httpSpy.put).toHaveBeenCalled();
  });

  it('should delete project', () => {
    httpSpy.delete.and.returnValue(of({}));
    service.deleteProject('id').subscribe();
    expect(httpSpy.delete).toHaveBeenCalled();
  });
});
