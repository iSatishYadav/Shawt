import { TestBed } from '@angular/core/testing';

import { LinkService } from './link.service';

describe('LinkServiceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LinkService = TestBed.get(LinkService);
    expect(service).toBeTruthy();
  });
});
