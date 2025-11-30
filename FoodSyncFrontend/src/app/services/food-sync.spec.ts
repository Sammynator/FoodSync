import { TestBed } from '@angular/core/testing';

import { FoodSync } from './food-sync';

describe('FoodSync', () => {
  let service: FoodSync;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FoodSync);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
