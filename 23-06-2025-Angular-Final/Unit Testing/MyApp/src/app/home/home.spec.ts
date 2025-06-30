import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { UserService } from '../services/User.service';
import { provideHttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Component } from '@angular/core';
import { of } from 'rxjs';
import { provideHttpClientTesting } from '@angular/common/http/testing';

class MockUserService {
  username$ = of('mockuser');
  GetUserProfile() {
    return of({
      firstName: 'Jane',
      lastName: 'Smith',
      email: 'jane.smith@example.com',
      username: 'janesmith'
    });
  }
}

const mockActivatedRoute = {
  snapshot: {
    paramMap: {
      get: (key: string) => key === 'un' ? 'testuser' : null
    }
  }
};

@Component({
  standalone: true,
  imports: [Home],
  template: `<app-home></app-home>`
})
class HostComponent {}

describe('Home', () => {
  let fixture: ComponentFixture<HostComponent>;
  let hostComponent: HostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent],
      providers: [
        { provide: UserService, useClass: MockUserService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the home component', () => {
    const homeComponent = fixture.debugElement.children[0].componentInstance;
    expect(homeComponent).toBeTruthy();
  });

  it('should load user profile from service', () => {
    const homeComponent = fixture.debugElement.children[0].componentInstance;
    expect((homeComponent as any).userProfile.firstName).toBe('Jane');
    expect((homeComponent as any).userProfile.lastName).toBe('Smith');
    expect((homeComponent as any).userProfile.email).toBe('jane.smith@example.com');
    expect((homeComponent as any).userProfile.username).toBe('janesmith');
  });
});
