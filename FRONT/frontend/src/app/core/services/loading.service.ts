import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingSignal = signal<boolean>(false);
  public isLoading = this.loadingSignal.asReadonly();

  show(): void {
    this.loadingSignal.set(true);
  }

  hide(): void {
    this.loadingSignal.set(false);
  }
}
