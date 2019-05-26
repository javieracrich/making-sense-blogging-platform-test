import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Service } from './service';
import { BaseEntity } from '../entities/base';
import { SearchPostFilter } from '../entities/search-post-filter';

export abstract class GenericService implements Service {
  constructor(
    protected http: HttpClient,
    protected resource: string,
    protected apiurl: string
  ) {}

  count(): Observable<number> {
    const url = `${this.apiurl}${this.resource}/count`;
    return this.http.get<number>(url);
  }

  getAll<T>(): Observable<T[]> {
    const url = `${this.apiurl}${this.resource}`;
    return this.http.get<T[]>(url);
  }

  get<T>(id: number): Observable<T> {
    const url = `${this.apiurl}${this.resource}/${id}`;
    return this.http.get<T>(url);
  }

  search<T>(filter: SearchPostFilter): Observable<T[]> {
    const url = `${this.apiurl}${this.resource}/search`;
    return this.http.post<T[]>(url, filter);
  }

  post(entity: BaseEntity) {
    const url = `${this.apiurl}${this.resource}/`;
    return this.http.post(url, entity);
  }

  put(entity: BaseEntity) {
    const url = `${this.apiurl}${this.resource}/${entity.id}`;
    return this.http.put(url, entity);
  }

  delete(id: number) {
    const url = `${this.apiurl}${this.resource}/${id}`;
    return this.http.delete(url);
  }
}
