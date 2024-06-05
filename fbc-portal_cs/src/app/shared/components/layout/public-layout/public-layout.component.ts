import { Component, OnInit, Renderer2, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-public-layout',
  templateUrl: './public-layout.component.html',
  styleUrls: ['./public-layout.component.css']
})
export class PublicLayoutComponent implements OnInit {

  constructor(private renderer: Renderer2,
    @Inject(DOCUMENT) private document: Document) { }

  ngOnInit() {
    this.setBodyClass();
  }

  rightbar(event) {
    const toggle = document.getElementById('sidenav-overlay');
    if (event.currentTarget.className === 'sidenav-overlay d-block') {
      this.renderer.removeClass(toggle, 'd-block');
      this.document.body.classList.remove('menu-open');
      this.document.body.classList.add('menu-close');
      this.renderer.addClass(toggle, 'd-none');
    } else if (event.currentTarget.className === 'sidenav-overlay d-none') {
      this.renderer.removeClass(toggle, 'd-none');
      this.document.body.classList.remove('menu-close');
      this.document.body.classList.add('menu-open');
      this.renderer.addClass(toggle, 'd-block');
    }
  }

  setBodyClass() {
    const previosBodyClassList = [].slice.call(document.body.classList);
    const self = this;
    previosBodyClassList.forEach(function (c) {
      self.renderer.removeClass(document.body, c);
    });
    const currentBodyClassList = ['vertical-layout', 'bg-full-screen-image', 'vertical-overlay-menu',
      '2-columns', 'pace-done', 'menu-close', 'fixed-navbar'];
    currentBodyClassList.forEach(function (c) {
      self.renderer.addClass(document.body, c);
    });
  }
}
