import { BaseEntity } from './base';
export class Post extends BaseEntity {
  text: string;
  date: string;
  author: string;
}
