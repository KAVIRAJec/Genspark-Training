import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Profile } from './profile';
import { UserService } from '../services/User.service';
import { of } from 'rxjs';
import { UserModel } from '../Models/UserModel';

class MockUserService {
  callGetProfile() {
    return of({
      id: 1,
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      username: 'johndoe'
    });
  }
}

describe('Profile', () => {
  let component: Profile;
  let fixture: ComponentFixture<Profile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [],
      providers: [
        { provide: UserService, useClass: MockUserService }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load profile data from service', () => {
    expect(component.profileData.firstName).toBe('John');
    expect(component.profileData.lastName).toBe('Doe');
    expect(component.profileData.email).toBe('john.doe@example.com');
    expect(component.profileData.username).toBe('johndoe');
  });
});
