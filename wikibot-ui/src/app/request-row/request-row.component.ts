import { Component, OnInit, ViewContainerRef, InjectionToken, Injector, Input } from '@angular/core';
import { Request } from '../request';
import { Overlay, OverlayConfig, OverlayRef} from '@angular/cdk/overlay';
import { RequestReviewPanelComponent, CONTAINER_DATA } from '../request-review-panel/request-review-panel.component';
import {
  ComponentPortal,
  // This import is only used to define a generic type. The current TypeScript version incorrectly
  // considers such imports as unused (https://github.com/Microsoft/TypeScript/issues/14953)
  // tslint:disable-next-line:no-unused-variable
  Portal,
  TemplatePortalDirective,
  CdkPortal,
  PortalInjector
} from '@angular/cdk/portal';

@Component({
  selector: 'app-request-row',
  templateUrl: './request-row.component.html',
  styleUrls: ['./request-row.component.css']
})

export class RequestRowComponent implements OnInit {
 @Input() request: Request
  ref: any;
  //request: Request = {
  //  id: 1,
  //  statusName: 'Approved',
  //  comment: 'Someone messed up',
  //  requestingUsername: 'Tigerpaw28',
  //  submittedDateUTC:  new Date('09/28/2020'),
  //  timePreStartedUTC: null,
  //  timeStartedUTC: null,
  //  timePreFinishedUTC: null,
  //  timeFinishedUTC: null,
  //  rawRequest: 'Convert a=>b',
  //  notes: 'Nada'
  //}
  constructor(public overlay: Overlay, public viewContainerRef: ViewContainerRef, private injector: Injector) {
   }

  ngOnInit() {
  }

  showOverlayForReview(id: number){
    let config = new OverlayConfig();
    
    config.width = '90%';
    config.height = '90%';
    config.positionStrategy = this.overlay.position()
        .global()
        .centerHorizontally()
        .centerVertically();

    config.hasBackdrop = true;

    let overlayRef = this.overlay.create(config);

    overlayRef.backdropClick().subscribe(() => {
      overlayRef.dispose();
    });

 
    const injectorTokens = new WeakMap();
    injectorTokens.set(CONTAINER_DATA, this.request);
    var portalinj = new PortalInjector(this.injector, injectorTokens);
    var cportal = new ComponentPortal(RequestReviewPanelComponent,this.viewContainerRef,portalinj);
    
    var component = cportal;
    this.ref = overlayRef.attach(component); 
    this.ref.instance.closePanel.subscribe(() => overlayRef.detach());
  }

}
