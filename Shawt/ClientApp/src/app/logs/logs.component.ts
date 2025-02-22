import { Component, OnInit } from '@angular/core';
import { LinkService } from '../services/link.service';
import { LoaderService } from '../services/loader.service';
import { ActivatedRoute } from '@angular/router';
import { LinkWithLog } from '../models/link-with-log';
import '../extensions/array';
import '../extensions/date';
import { Log } from '../models/log';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.css']
})
export class LogsComponent implements OnInit {

  constructor(private linkService: LinkService, private loader: LoaderService, private route: ActivatedRoute) { }

  link: LinkWithLog;
  logs: Log[];

  osPieData: any[];
  browserPieData: any[];
  timeSeries: any;
  chartTitle: string = 'Clicks';


  legend: boolean = false;
  showLabels: boolean = true;
  animations: boolean = true;
  xAxis: boolean = true;
  yAxis: boolean = true;
  showYAxisLabel: boolean = true;
  showXAxisLabel: boolean = false;
  xAxisLabel: string = 'Date';
  yAxisLabel: string = 'Clicks';
  timeline: boolean = true;

  ngOnInit() {
    this.loader.show();
    const id = this.route.snapshot.paramMap.get('id');
    this.getLinkWithLog(id);
  }
  getLinkWithLog(id: string) {
    this.linkService.getLinkWithLogs(id)
      .subscribe(link => {
        console.log('[Logs] Link', link);
        this.link = link;
        this.logs = this.link.logs;
        this.browserPieData = this.getBrowserSummary(this.logs);
        this.osPieData = this.getOsSummary(this.logs);
        this.timeSeries = this.getTimeSeries(this.logs);
        this.loader.hide();
      });
  }
  private getTimeSeries(logs: Log[]): any {

    var rawDateStringAndClicks = logs
      .map(x => ({
        date: new Date(x.timestamp).toFormatString("MM/dd/yyyy"),
        click: 1
      }));
    var dateStringWiseClicks = rawDateStringAndClicks.groupByCount("date");
    const dateWiseClicks = Object.keys(dateStringWiseClicks).map((date) => {
      return {
        name: date,
        value: dateStringWiseClicks[date]
      };
    });

    var linksForTimeSeries = [
      {
        "name": "Clicks",
        series: dateWiseClicks.map(x => ({ name: new Date(x.name), value: x.value }))
      }
    ];
    //console.log

    return linksForTimeSeries;
  }

  private getBrowserSummary(logs: Log[]) {
    const browsers = logs.map(x => ({ browser: x.browser.split(' ', 1)[0] })).groupByCount('browser');
    const browserWiseCount = [];
    Object.keys(browsers).map(k => {
      browserWiseCount.push({ name: k, value: browsers[k] });
      return k;
    });
    return browserWiseCount;
  }

  private getOsSummary(logs: Log[]) {
    const oss: any = logs.map(x => ({ os: x.os.split(' ', 1)[0] })).groupByCount('os');
    const osWiseCount = [];
    Object.keys(oss).map(k => {
      osWiseCount.push({ name: k, value: oss[k] });
      return k;
    });
    return osWiseCount;
  }
}
