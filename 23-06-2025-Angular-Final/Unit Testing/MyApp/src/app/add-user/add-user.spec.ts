import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUser } from './add-user';
import { UserService } from '../services/User.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideMockStore } from '@ngrx/store/testing';
import { provideStore } from '@ngrx/store';

describe('AddUser', () => {
  let component: AddUser;
  let fixture: ComponentFixture<AddUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [
        UserService,
        provideHttpClientTesting(),
        provideStore({}),
        provideMockStore({ initialState: {} })
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
