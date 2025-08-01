import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { LoginDto, RegisterDto } from '../../models/auth.model';
import AOS from 'aos';
import { AsyncPipe, CommonModule } from '@angular/common';
import { ToastService } from '../../Components/Toast/toast.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-authentication',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './authentication.html',
  styleUrl: './authentication.css'
})
export class Authentication {
  authForm: FormGroup;
  isLoginMode = true;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toast: ToastService,
    private router: Router
  ) {
    this.authForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      userName: [''],
      firstName: [''],
      lastName: [''],
      phone: [''],
      address: ['']
    });
  }

  ngOnInit() {
    AOS.init();
  }

  switchMode() {
    this.isLoginMode = !this.isLoginMode;
    setTimeout(() => AOS.refresh(), 300);
    this.authForm.reset();
  }

  onSubmit() {
    if (this.authForm.invalid) {
      this.toast.show('Please fill all required fields correctly.', 'error');
      return;
    }
    this.loading = true;
    if (this.isLoginMode) {
      const loginDto: LoginDto = {
        email: this.authForm.value.email,
        password: this.authForm.value.password
      };
      this.authService.login(loginDto).subscribe({
        next: res => {
          console.log('Login successful:', res);
          this.toast.show('Login successful!', 'success');
          this.loading = false;
          this.router.navigate(['/home']);
        },
        error: err => {
          this.toast.show(err?.error?.message || 'Login failed', 'error');
          this.loading = false;
        }
      });
    } else {
      const registerDto: RegisterDto = {
        userName: this.authForm.value.userName,
        email: this.authForm.value.email,
        password: this.authForm.value.password,
        firstName: this.authForm.value.firstName,
        lastName: this.authForm.value.lastName,
        phone: this.authForm.value.phone,
        address: this.authForm.value.address
      };
      this.authService.register(registerDto).subscribe({
        next: res => {
          this.toast.show('Registration successful!', 'success');
          this.loading = false;
          this.router.navigate(['/home']);
        },
        error: err => {
          this.toast.show(err?.error?.message || 'Registration failed', 'error');
          this.loading = false;
        }
      });
    }
  }

  logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.toast.show('Logged out successfully!', 'success');
      },
      error: () => {
        this.toast.show('Logout failed', 'error');
      }
    });
  }
}
