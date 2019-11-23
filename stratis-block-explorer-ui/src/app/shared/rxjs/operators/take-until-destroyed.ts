import { ReplaySubject, Observable, MonoTypeOperatorFunction } from 'rxjs';
import { OnDestroy } from '@angular/core';
import { takeUntil, tap } from 'rxjs/operators';


/**
 * Emits the values emitted by the source Observable until the subscriber component gets destroyed.
 * @param {ISubscriber} subscriber The Subscriber whose will cause the observable to stop emitting values when destroyed.
 * @return {Observable<T>} An Observable that emits the values from the source
 * Observable until such time as `subscriber` gets destroyed.
 * @method takeUntil
 * @owner Observable
 */
export function takeUntilDestroyed<T>(subscriber: OnDestroy, debug: boolean = false): MonoTypeOperatorFunction<T> {
    if (!subscriber) {
        throw new Error("subscriber must be a valid instance.");
    }

    let __untilDestroyedSubscription: ReplaySubject<boolean> = subscriber["__untilDestroyedSubscription"];
    if (!__untilDestroyedSubscription) {
        if (debug) {
            console.debug("__untilDestroyedSubscription emitter not found, creating one...");
        }

        __untilDestroyedSubscription = new ReplaySubject<boolean>();
        subscriber["__untilDestroyedSubscription"] = __untilDestroyedSubscription;

        if (debug) {
            console.debug("replacing ngOnDestroy implementation (will call original one at the end)");
        }

        subscriber.ngOnDestroy = ngOnDestroyFactory(__untilDestroyedSubscription, subscriber.ngOnDestroy, debug);
    }

    if (debug) {
        console.debug("relaying takeUntil");
    }

    if (debug) {
        let observer = __untilDestroyedSubscription.pipe(tap(destroyed => console.debug("triggering takeUntil")));
        return takeUntil(observer);
    }
    else {
        return takeUntil(__untilDestroyedSubscription);
    }
}

function ngOnDestroyFactory(componentDestroyed$: ReplaySubject<boolean>, originalNgOnDestroy: () => void, debug): () => void {
    if (debug) {
        console.debug("creating custom ngOnDestroy to trigger inner takeUntil.");
    }

    return () => {
        if (debug) {
            console.debug("destroying component, emitting destroy$.");
        }

        componentDestroyed$.next(true);
        componentDestroyed$.complete();

        if (debug) {
            console.debug("destroying component, calling original ngOnDestroy.");
        }
        originalNgOnDestroy();
        if (debug) {
            console.debug("destroying component, original ngOnDestroy called.");
        }
    }
}
