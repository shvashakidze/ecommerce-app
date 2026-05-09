import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
})
export class RegisterComponent {
  firstName = '';
  lastName = '';
  email = '';
  password = '';
  error = signal('');
  loading = signal(false);

  constructor(private authService: AuthService) {}

  onSubmit() {
    this.loading.set(true);
    this.error.set('');

    this.authService.register({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: (res) => {
        this.authService.setUser(res);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        if (err.status === 400) {
          this.error.set(err.error || 'Email უკვე გამოყენებულია');
        } else {
          this.error.set('რეგისტრაცია ვერ მოხერხდა, სცადე თავიდან');
        }
      }
    });
  }
}