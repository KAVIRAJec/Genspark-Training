import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import * as ClientActions from '../../NgRx/Client/client.actions';
import * as ProjectActions from '../../NgRx/Project/project.actions';
import * as FreelancerActions from '../../NgRx/Freelancer/freelancer.actions';

describe('Home', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;
  let storeSpy: jasmine.SpyObj<Store<any>>;

  beforeEach(async () => {
    storeSpy = jasmine.createSpyObj('Store', ['dispatch', 'select']);
    storeSpy.select.and.returnValue(of({ totalRecords: 10 }));
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [
        { provide: Store, useValue: storeSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should dispatch load actions on init', () => {
    expect(storeSpy.dispatch).toHaveBeenCalledWith(ClientActions.loadClients({ page: 1, pageSize: 1 }));
    expect(storeSpy.dispatch).toHaveBeenCalledWith(ProjectActions.loadProjects({ page: 1, pageSize: 1 }));
    expect(storeSpy.dispatch).toHaveBeenCalledWith(FreelancerActions.loadFreelancers({ page: 1, pageSize: 1 }));
  });

  it('should update animated counts when pagination changes', () => {
    component['clientCountTarget'] = 5;
    component['projectCountTarget'] = 7;
    component['freelancerCountTarget'] = 9;
    component['hasAnimated'] = false;
    // Mock countSection and its nativeElement.getBoundingClientRect
    component['countSection'] = {
      nativeElement: {
        getBoundingClientRect: () => ({ top: 0, bottom: window.innerHeight - 1 })
      }
    };
    component['tryAnimateCounts']();
    expect(component['hasAnimated']).toBeTrue();
  });
});
