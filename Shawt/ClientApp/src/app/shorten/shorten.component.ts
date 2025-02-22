import { Component, OnInit } from '@angular/core';
import { LinkService } from '../services/link.service';
import { Link } from '../models/link';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { LoaderService } from '../services/loader.service';

@Component({
  selector: 'app-shorten',
  templateUrl: './shorten.component.html',
  styleUrls: ['./shorten.component.css']
})
export class ShortenComponent implements OnInit {

  constructor(private linkService: LinkService, private loader: LoaderService) { }
  model = new Link();
  linkForm = new FormGroup({
    link: new FormControl('', { validators: [Validators.required] })
  });
  error: string;

  ngOnInit() {
  }

  onLinkFormSubmit() {
    console.log(this.linkForm.valid);
    console.log(this.linkForm.value);
    this.error = null;
    this.loader.show();

    this.linkService.shortenLink(JSON.stringify({ url: this.linkForm.get('link').value }))
      .subscribe(result => {
        console.log("links", result);
        this.linkForm.get('link').setValue('');
        this.model.shortLink = result.shortLink;
        this.loader.hide();
      }, error => {
        this.loader.hide();
        this.model.shortLink = '';
        console.error(error);
        console.error(error.status);
        if (error.status && error.status == 400) {
          if (error.error.errors.Url) {
            //console.error("error.error.errors.Url", error.error.errors.Url);
            //console.error("error.error.errors.Url[0]", error.error.errors.Url[0]);
            if (error.error.errors.Url.length > 0) {
              this.error = error.error.errors.Url[0];
            }
            else {
              this.error = "Invalid URL";
            }
            this.linkForm.get('link').setErrors([this.error]);
          }
        }
      });
  }
}
