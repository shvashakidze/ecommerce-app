import { Injectable, signal, computed } from '@angular/core';
 
export interface CartItem {
  id: string; name: string; price: number;
  quantity: number; imageUrl: string;
}
 
@Injectable({ providedIn: 'root' })
export class CartService {
  items = signal<CartItem[]>(
    JSON.parse(localStorage.getItem('cart') || '[]')
  );
 
  total = computed(() =>
    this.items().reduce((sum, i) => sum + i.price * i.quantity, 0)
  );
 
  itemCount = computed(() =>
    this.items().reduce((sum, i) => sum + i.quantity, 0)
  );
 
  addItem(product: any) {
    this.items.update(items => {
      const existing = items.find(i => i.id === product.id);
      const updated = existing
        ? items.map(i => i.id === product.id ? { ...i, quantity: i.quantity + 1 } : i)
        : [...items, { ...product, quantity: 1 }];
      localStorage.setItem('cart', JSON.stringify(updated));
      return updated;
    });
  }
 
  removeItem(id: string) {
    this.items.update(items => {
      const updated = items.filter(i => i.id !== id);
      localStorage.setItem('cart', JSON.stringify(updated));
      return updated;
    });
  }
 
  clear() {
    this.items.set([]);
    localStorage.removeItem('cart');
  }
}