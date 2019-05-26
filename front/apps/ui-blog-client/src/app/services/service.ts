import { Observable } from 'rxjs';
import { BaseEntity } from '../entities/base';

export interface Service {
  getAll();
  get(id: number);
  count(): Observable<number>;
  post(entity: BaseEntity);
  put(entity: BaseEntity);
  delete(id: number);

}
