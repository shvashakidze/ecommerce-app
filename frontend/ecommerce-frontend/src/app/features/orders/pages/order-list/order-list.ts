import { Component, OnInit, signal } from '@angular/core';
import { DecimalPipe, DatePipe, NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-order-list',
  imports: [DecimalPipe, DatePipe, RouterLink, NgClass],
  templateUrl: './order-list.html',
})
export class OrderListComponent implements OnInit {
  orders = signal<any[]>([]);
  loading = signal(true);

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any[]>(`${environment.apiUrl}/orders`).subscribe({
      next: (res) => {
        this.orders.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  getStatusLabel(status: number) {
    const statuses: any = {
      0: 'მომლოდინე',
      1: 'დადასტურებული',
      2: 'გაგზავნილი',
      3: 'მიწოდებული',
      4: 'გაუქმებული'
    };
    return statuses[status] ?? 'უცნობი';
  }

  getStatusClass(status: number) {
    const classes: any = {
      0: 'bg-yellow-100 text-yellow-700',
      1: 'bg-blue-100 text-blue-700',
      2: 'bg-purple-100 text-purple-700',
      3: 'bg-green-100 text-green-700',
      4: 'bg-red-100 text-red-700'
    };
    return classes[status] ?? 'bg-gray-100 text-gray-700';
  }
}