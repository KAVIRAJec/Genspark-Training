import { AsyncPipe, NgIf } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CreateClientModel } from '../../Models/Client.model';
import { CreateFreelancerModel } from '../../Models/Freelancer.model';
import { AuthenticationService } from '../../Services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Actions, ofType } from '@ngrx/effects';
import * as ClientActions from '../../NgRx/Client/client.actions';
import * as FreelancerActions from '../../NgRx/Freelancer/freelancer.actions';
import { selectClientError, selectClientLoading } from '../../NgRx/Client/client.selector';
import { selectFreelancerError, selectFreelancerLoading } from '../../NgRx/Freelancer/freelancer.selector';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-auth',
  imports: [ReactiveFormsModule, NgIf, AsyncPipe],
  templateUrl: './auth.html',
  styleUrl: './auth.css'
})
export class Auth implements OnInit, OnDestroy {
  isFlipped = false;
  loginForm: FormGroup;
  registerForm: FormGroup;
  role: 'client' | 'freelancer' = 'client';
  store = inject(Store);
  private actions$ = inject(Actions);
  loading$ = this.store.select(selectClientLoading);
  error$ = this.store.select(selectClientError);
  private destroy$ = new Subject<void>();
  loginLoading = false;

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['mode'] === 'register') {
        this.flipCard(new Event(''));
      }
    });
  }

  constructor(private fb: FormBuilder, private AuthService: AuthenticationService, private toastr: ToastrService, private router: Router, private route: ActivatedRoute) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
    this.registerForm = this.createRegisterForm('client');

    // Listen for registration success actions
    this.actions$
      .pipe(
        ofType(ClientActions.addClientSuccess, FreelancerActions.addFreelancerSuccess),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.toastr.success('Registration successful!', 'Success');
        this.isFlipped = false; // Optionally flip back to login
        this.registerForm.reset();
      });
  }

  flipCard(event: Event) {
    event.preventDefault();
    this.isFlipped = !this.isFlipped;
  }

  onLogin() {
    if (this.loginForm.valid) {
      this.loginLoading = true;
      this.AuthService.login(this.loginForm.value.email, this.loginForm.value.password).subscribe({
        next: (response) => {
          this.loginLoading = false;
          this.toastr.success('Login successful!', 'Success');
          this.router.navigate(['/home']);
        },
        error: (error) => {
          this.loginLoading = false;
          // Extract the first validation error message from the error object
          let errorMessage = 'Login failed. Please try again.';
          if (error?.errors && typeof error.errors === 'object') {
            const firstKey = Object.keys(error.errors)[0];
            if (error && typeof error === 'object' && 'message' in error) {
              this.toastr.error((error as any).message, 'Error');
            } else if (firstKey && Array.isArray(error.errors[firstKey]) && error.errors[firstKey].length > 0) {
              errorMessage = error.errors[firstKey][0];
              this.toastr.error(errorMessage, 'Error');
            }
          }
        }
      });
    } else {
      this.loginForm.markAllAsTouched();
    }
    this.registerForm.reset();
  }

  onRoleChange(role: 'client' | 'freelancer') {
    this.role = role;
    this.registerForm = this.createRegisterForm(role);
    if (role === 'client') {
      this.loading$ = this.store.select(selectClientLoading);
      this.error$ = this.store.select(selectClientError);
    } else {
      this.loading$ = this.store.select(selectFreelancerLoading);
      this.error$ = this.store.select(selectFreelancerError);
    }
  }

  onRegister() {
    if (this.registerForm.valid) {
      const data = this.registerForm.value;
      if (this.role === 'client') {
          this.store.dispatch(ClientActions.addClient({ client: data as CreateClientModel }));
        } else {
          this.store.dispatch(FreelancerActions.addFreelancer({ freelancer: data as CreateFreelancerModel }));
        }
        // Extract error message from the store's latest error state
        let errorMessage = 'Registration failed. Please try again.';
        this.error$.subscribe(error => {
          if (error && typeof error === 'object' && 'message' in error) {
            this.toastr.error((error as any).message, 'Error');
          } else if (error && typeof error === 'object' && 'errors' in error) {
            console.log("err", error)
            const firstKey = Object.keys((error as any).errors)[0];
            if (firstKey && Array.isArray((error as any).errors[firstKey]) && (error as any).errors[firstKey].length > 0) {
              errorMessage = (error as any).errors[firstKey][0];
              this.toastr.error(errorMessage, 'Error')
            }
          } else if (typeof error === 'string') {
            this.toastr.error(error, 'Error');
          }
        });

        
    }
        
    this.loginForm.reset();
  }

  createRegisterForm(role: 'client' | 'freelancer'): FormGroup {
    if (role === 'client') {
      return this.fb.group({
        username: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        companyName: [''],
        location: [''],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
        role: ['client']
      }, { validators: this.passwordMatchValidator });
    } else { //company & skills are not included in the freelancer form
      return this.fb.group({
        username: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        experienceYears: ['', [Validators.required, Validators.min(0)]],
        hourlyRate: ['', [Validators.required, Validators.min(0)]],
        location: [''],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
        role: ['freelancer']
      }, { validators: this.passwordMatchValidator });
    }
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  get email() { return this.loginForm.get('email'); }
  get password() { return this.loginForm.get('password'); }
  get regUsername() { return this.registerForm.get('username'); }
  get regEmail() { return this.registerForm.get('email'); }
  get regPassword() { return this.registerForm.get('password'); }
  get regConfirmPassword() { return this.registerForm.get('confirmPassword'); }
  get regCompanyName() { return this.registerForm.get('companyName'); }
  get regLocation() { return this.registerForm.get('location'); }
  get regExperienceYears() { return this.registerForm.get('experienceYears'); }
  get regHourlyRate() { return this.registerForm.get('hourlyRate'); }
  get regRole() { return this.registerForm.get('role'); }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
