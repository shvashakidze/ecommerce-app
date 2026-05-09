import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
 
@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = `${environment.apiUrl}/products`;
 
  constructor(private http: HttpClient) {}
 
  getAll(params?: { search?: string; categoryId?: string; page?: number }) {
    let httpParams = new HttpParams();
    if (params?.search) httpParams = httpParams.set('search', params.search);
    if (params?.categoryId) httpParams = httpParams.set('categoryId', params.categoryId);
    if (params?.page) httpParams = httpParams.set('page', params.page.toString());
    return this.http.get<any>(this.apiUrl, { params: httpParams });
  }
 
  getById(id: string) {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
 
  create(product: any) {
    return this.http.post<any>(this.apiUrl, product);
  }
}