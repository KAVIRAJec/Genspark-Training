import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Auth } from './auth';
import { Store } from '@ngrx/store';
import { Actions } from '@ngrx/effects';
import { of, Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { AuthenticationService } from '../../Services/auth.service';

describe('Auth', () => {
  let component: Auth;
  let fixture: ComponentFixture<Auth>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Auth],
      providers: [
        { provide: Store, useValue: { select: () => of(), dispatch: () => {} } },
        { provide: Actions, useValue: new Subject() },
        { provide: ToastrService, useValue: { success: () => {}, error: () => {} } },
        { provide: ActivatedRoute, useValue: { queryParams: of({}) } },
        { provide: Router, useValue: { navigate: () => {} } },
        { provide: FormBuilder, useClass: FormBuilder },
        { provide: AuthenticationService, useValue: {} }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Auth);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should flip the card when flipCard is called', () => {
    expect(component.isFlipped).toBeFalse();
    component.flipCard(new Event(''));
    expect(component.isFlipped).toBeTrue();
    component.flipCard(new Event(''));
    expect(component.isFlipped).toBeFalse();
  });

  it('should have invalid loginForm if empty', () => {
    component.loginForm.setValue({ email: '', password: '' });
    expect(component.loginForm.invalid).toBeTrue();
  });

  it('should have valid loginForm with valid values', () => {
    component.loginForm.setValue({ email: 'test@mail.com', password: '123456' });
    expect(component.loginForm.valid).toBeTrue();
  });

  it('should have invalid registerForm if required fields are empty', () => {
    component.registerForm.reset();
    expect(component.registerForm.invalid).toBeTrue();
  });

  it('should flip card on ngOnInit if mode=register in queryParams', () => {
    // Recreate component with mode=register
    const route = TestBed.inject(ActivatedRoute);
    (route as any).queryParams = of({ mode: 'register' });
    const fixture2 = TestBed.createComponent(Auth);
    const component2 = fixture2.componentInstance;
    spyOn(component2, 'flipCard');
    component2.ngOnInit();
    expect(component2.flipCard).toHaveBeenCalled();
  });
});
