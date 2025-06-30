import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserList } from './user-list';
import { provideMockStore } from '@ngrx/store/testing';
import { provideStore } from '@ngrx/store';

describe('UserList', () => {
  let component: UserList;
  let fixture: ComponentFixture<UserList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserList],
      providers: [provideStore({}), provideMockStore({ initialState: {} })]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
