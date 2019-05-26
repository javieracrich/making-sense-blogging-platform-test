import { PostService } from './../services/post.service';
import { BlogService } from './../services/blog.service';
import { Component, OnInit } from '@angular/core';
import { Blog } from '../entities/blog';
import { Post } from '../entities/post';
import { Observable } from 'rxjs';

@Component({
  selector: 'blogging-client-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  selectedBlog: Blog;
  blogs: Observable<Blog[]>;
  posts: Observable<Post[]>;

  constructor(
    private blogService: BlogService,
    private postService: PostService
  ) {}

  ngOnInit() {
    this.blogs = this.blogService.getAll<Blog>();
  }

  onBlogSelected() {
    this.posts = this.postService.getPosts(this.selectedBlog);
  }
}
