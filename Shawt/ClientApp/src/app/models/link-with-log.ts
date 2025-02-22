import { Log } from "./log";

export class LinkWithLog {
  linkId: string;
  originalLink: string;
  shortLink: string;
  createdOn: Date;
  clicks: number;
  logs: Log[];
}
