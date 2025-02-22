import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  constructor(private ngxSpinner: NgxSpinnerService) { }

  show() {
    this.ngxSpinner.show();
  }

  hide() {
    this.ngxSpinner.hide();
  }
}
