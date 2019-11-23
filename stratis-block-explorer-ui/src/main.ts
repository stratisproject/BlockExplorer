import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import 'hammerjs';
import { setup, track, printSubscribers } from 'observable-profiler';


if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic()
    .bootstrapModule(AppModule)
    .then(ref => {
        if (environment.production == false) {
            track();
            window["stopProfiler"] = () => {
                ref.destroy();
                const subscribers = track(false);
                printSubscribers({
                    subscribers,
                });
            }
            (window as any).printProfiler = () => {
                const subscribers = track(false);
                printSubscribers({
                    subscribers,
                });
                track();
            }
        }
    })
    .catch(err => console.error(err));
