import { Injectable, Inject } from '@angular/core';
import { GenericService } from './generic.service';
import { HttpClient } from '@angular/common/http';
import { Post } from '../entities/post';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PostService extends GenericService {
  constructor(http: HttpClient, @Inject('apiurl') apiurl: string) {
    super(http, 'post', apiurl);
  }

  getPosts(blogId): Observable<Post[]> {
    const url = `${this.apiurl}${this.resource}/blog/${blogId}`;
    return this.http.get<Post[]>(url);
  }
}
