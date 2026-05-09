import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
})
export class LoginComponent {
  email = '';
  password = '';
  error = signal('');
  loading = signal(false);

  constructor(private authService: AuthService) {}

  onSubmit() {
    this.loading.set(true);
    this.error.set('');

    this.authService.login(this.email, this.password).subscribe({
      next: (res) => {
        this.authService.setUser(res);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        if (err.status === 401) {
          this.error.set('Email ან პაროლი არასწორია');
        } else if (err.status === 400) {
          this.error.set('შეავსე ყველა ველი სწორად');
        } else {
          this.error.set('სერვერის შეცდომა, სცადე თავიდან');
        }
      }
    });
  }
}