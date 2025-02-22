import { Component, OnInit } from '@angular/core';
import { LinkService } from '../services/link.service';
import { Link } from '../models/link';
import { LoaderService } from '../services/loader.service';

@Component({
  selector: 'app-links',
  templateUrl: './links.component.html',
  styleUrls: ['./links.component.scss']
})
export class LinksComponent implements OnInit {
  private linksSummaryCount: number = 10;
  constructor(private linkService: LinkService, private loader: LoaderService) { }
  graphData = [{ 'name': 'init', 'value': 0 }];
  links: Link[];

  colorScheme = {
    domain: ['#f44336', '#2196F3', '#9C27B0', '#00BCD4', '#00BCD4', '#4CAF50', '#CDDC39', '#FFEB3B', '#795548', '#9E9E9E']
  };

  ngOnInit() {
    this.loader.show();
    this.getAllLinks();
  }
  getAllLinks() {
    this.linkService.getLinks().subscribe(x => {
      this.links = x;
      let linksForGraph = this.links.sort((x, y) => y.clicks - x.clicks).slice(0, this.linksSummaryCount);
      this.graphData = linksForGraph.map((value) => {
        return { name: value.originalLink, value: value.clicks }
      });
      this.loader.hide();
      console.log("[Links Component] Links", this.links);
    });
  }
  shortLinkClicked(link: Link): void {
    //link.clicks++;
  }
}
