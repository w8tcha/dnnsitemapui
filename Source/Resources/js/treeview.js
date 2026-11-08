/*
 * Treeview - Vanilla JS port of the jQuery Treeview plugin (1.5pre)
 * Hides and shows branches of a tree. No dependencies (no jQuery, no jquery.cookie).
 *
 * Based on http://bassistance.de/jquery-plugins/jquery-plugin-treeview/
 * Copyright (c) 2007 Jörn Zaefferer
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Usage:
 *   TreeView.init(".mytree", { collapsed: true, unique: false, animated: "fast", persist: "cookie", cookieId: "tree1" });
 *
 * Options:
 *   collapsed     - collapse all branches on start (default false)
 *   unique        - only one branch per level may be open (default false)
 *   animated      - false | "slow" | "normal" | "fast" | duration in ms
 *   persist       - "cookie" | "location"
 *   cookieId      - name of the cookie used with persist: "cookie" (default "treeview")
 *   cookieOptions - { expires: days|Date, path, domain, secure, sameSite }
 *   prerendered   - markup already contains all classes and hitareas
 *   control       - selector/element of a container holding three links: collapse all, expand all, toggle all
 *   toggle        - callback invoked after a branch was toggled, "this" is the <li>
 */
(function (window, document) {
    "use strict";

    // classes used by the plugin, need to be styled via external stylesheet
    var CLASSES = {
        open: "open",
        closed: "closed",
        expandable: "expandable",
        expandableHitarea: "expandable-hitarea",
        lastExpandableHitarea: "lastExpandable-hitarea",
        collapsable: "collapsable",
        collapsableHitarea: "collapsable-hitarea",
        lastCollapsableHitarea: "lastCollapsable-hitarea",
        lastCollapsable: "lastCollapsable",
        lastExpandable: "lastExpandable",
        last: "last",
        hitarea: "hitarea"
    };

    var SPEEDS = { slow: 600, normal: 400, fast: 200 };

    /* ---------------------------------------------------------------- cookie */

    function readCookie(name) {
        if (!document.cookie) {
            return null;
        }
        var cookies = document.cookie.split(";");
        for (var i = 0; i < cookies.length; i++) {
            var cookie = cookies[i].trim();
            if (cookie.substring(0, name.length + 1) === name + "=") {
                try {
                    return decodeURIComponent(cookie.substring(name.length + 1));
                } catch (e) {
                    return null;
                }
            }
        }
        return null;
    }

    function writeCookie(name, value, options) {
        options = options || {};
        var expires = options.expires;
        if (value === null) {
            value = "";
            expires = -1;
        }
        var parts = [name + "=" + encodeURIComponent(value)];
        if (typeof expires === "number" || (expires && expires.toUTCString)) {
            var date = expires;
            if (typeof expires === "number") {
                date = new Date();
                date.setTime(date.getTime() + expires * 24 * 60 * 60 * 1000);
            }
            parts.push("expires=" + date.toUTCString());
        }
        if (options.path) {
            parts.push("path=" + options.path);
        }
        if (options.domain) {
            parts.push("domain=" + options.domain);
        }
        if (options.sameSite) {
            parts.push("samesite=" + options.sameSite);
        }
        if (options.secure) {
            parts.push("secure");
        }
        document.cookie = parts.join("; ");
    }

    /**
     * cookie(name)                  -> read value (string or null)
     * cookie(name, value[, options]) -> write value, value null deletes the cookie
     */
    function cookie(name, value, options) {
        return typeof value === "undefined" ? readCookie(name) : writeCookie(name, value, options);
    }

    /* --------------------------------------------------------------- helpers */

    function toArray(list) {
        return Array.prototype.slice.call(list || []);
    }

    function resolve(target) {
        if (!target) {
            return [];
        }
        if (typeof target === "string") {
            return toArray(document.querySelectorAll(target));
        }
        if (target.nodeType === 1) {
            return [target];
        }
        // NodeList, HTMLCollection, Array, jQuery object
        return toArray(target).filter(function (el) {
            return el && el.nodeType === 1;
        });
    }

    function unique(list) {
        var result = [];
        list.forEach(function (el) {
            if (result.indexOf(el) === -1) {
                result.push(el);
            }
        });
        return result;
    }

    function childrenOf(el, tagOrClass) {
        return toArray(el.children).filter(function (child) {
            return tagOrClass.charAt(0) === "."
                ? child.classList.contains(tagOrClass.substring(1))
                : child.tagName === tagOrClass;
        });
    }

    function childLists(li) {
        return childrenOf(li, "UL");
    }

    function childHitareas(li) {
        return childrenOf(li, "." + CLASSES.hitarea);
    }

    function isHidden(el) {
        return window.getComputedStyle(el).display === "none";
    }

    function show(el) {
        el.style.display = "";
        if (isHidden(el)) {
            el.style.display = "block";
        }
    }

    function hide(el) {
        el.style.display = "none";
    }

    function hasHiddenList(li) {
        return childLists(li).some(isHidden);
    }

    function hasVisibleList(li) {
        return childLists(li).some(function (ul) {
            return !isHidden(ul);
        });
    }

    function swapClass(el, c1, c2) {
        var cl = el.classList;
        var has1 = cl.contains(c1);
        var has2 = cl.contains(c2);
        if (has2) {
            cl.remove(c2);
            cl.add(c1);
        }
        if (has1) {
            cl.remove(c1);
            cl.add(c2);
        }
    }

    function replaceClass(el, c1, c2) {
        if (el.classList.contains(c1)) {
            el.classList.remove(c1);
            el.classList.add(c2);
        }
    }

    function siblingsOf(el) {
        return el.parentElement
            ? toArray(el.parentElement.children).filter(function (child) {
                  return child !== el;
              })
            : [];
    }

    function duration(animated) {
        if (typeof animated === "number") {
            return animated;
        }
        return SPEEDS[animated] || SPEEDS.normal;
    }

    /* ------------------------------------------------------------- animation */

    function finishSlide(el, visible, callback) {
        el._treeviewAnimation = null;
        el.style.height = "";
        el.style.overflow = "";
        el.style.boxSizing = "";
        if (!visible) {
            hide(el);
        }
        if (callback) {
            callback.call(el);
        }
    }

    function slide(el, visible, animated, callback) {
        var running = el._treeviewAnimation;
        var canAnimate = animated && typeof el.animate === "function";

        if (!canAnimate) {
            if (running) {
                running.cancel();
                el._treeviewAnimation = null;
                el.style.height = el.style.overflow = el.style.boxSizing = "";
            }
            if (visible) {
                show(el);
            } else {
                hide(el);
            }
            if (callback) {
                callback.call(el);
            }
            return;
        }

        // start from the current (possibly mid-animation) height
        var from = isHidden(el) ? 0 : el.getBoundingClientRect().height;
        if (running) {
            running.cancel();
        }

        show(el);
        el.style.height = "";
        el.style.boxSizing = "border-box";
        el.style.overflow = "hidden";
        var to = visible ? el.getBoundingClientRect().height : 0;

        if (from === to) {
            finishSlide(el, visible, callback);
            return;
        }

        var animation = el.animate(
            [{ height: from + "px" }, { height: to + "px" }],
            { duration: duration(animated), easing: "ease-in-out" }
        );
        el._treeviewAnimation = animation;
        animation.onfinish = function () {
            if (el._treeviewAnimation === animation) {
                finishSlide(el, visible, callback);
            }
        };
    }

    function heightToggle(el, animated, callback) {
        // while animating, toggle relative to the animation's target state
        var running = el._treeviewAnimation;
        var currentlyVisible = running ? running._treeviewTarget : !isHidden(el);
        slide(el, !currentlyVisible, animated, callback);
        if (el._treeviewAnimation) {
            el._treeviewAnimation._treeviewTarget = !currentlyVisible;
        }
    }

    function heightHide(el, animated, callback) {
        slide(el, false, animated, callback);
        if (el._treeviewAnimation) {
            el._treeviewAnimation._treeviewTarget = false;
        }
    }

    function hoverClass(el, className) {
        if (el._treeviewHover) {
            return;
        }
        el._treeviewHover = true;
        el.addEventListener("mouseenter", function () {
            el.classList.add(className);
        });
        el.addEventListener("mouseleave", function () {
            el.classList.remove(className);
        });
    }

    /* ------------------------------------------------------------- treeview */

    function prepareBranches(items, settings) {
        if (!settings.prerendered) {
            items.forEach(function (li) {
                // mark last tree items
                if (!li.nextElementSibling) {
                    li.classList.add(CLASSES.last);
                }
                // collapse whole tree, or only those marked as closed, anyway except those marked as open
                var shouldCollapse = settings.collapsed || li.classList.contains(CLASSES.closed);
                if (shouldCollapse && !li.classList.contains(CLASSES.open)) {
                    childLists(li).forEach(hide);
                }
            });
        }
        // return all items with sublists
        return items.filter(function (li) {
            return childLists(li).length > 0;
        });
    }

    function applyClasses(branches, settings, toggler) {
        var hoverTargets = [];

        branches.forEach(function (li) {
            if (childrenOf(li, "A").length === 0) {
                childrenOf(li, "SPAN").forEach(function (span) {
                    if (span._treeviewClick) {
                        span.removeEventListener("click", span._treeviewClick);
                    }
                    span._treeviewClick = function (event) {
                        // don't handle click events on children, eg. checkboxes
                        if (span === event.target && span.nextElementSibling) {
                            toggler([span.nextElementSibling]);
                        }
                    };
                    span.addEventListener("click", span._treeviewClick);
                    hoverTargets.push(span);
                });
            }
            hoverTargets.push.apply(hoverTargets, toArray(li.querySelectorAll("a")));
        });

        unique(hoverTargets).forEach(function (el) {
            hoverClass(el, "hover");
        });

        var findHitareas = function () {
            var found = [];
            branches.forEach(function (li) {
                found.push.apply(found, toArray(li.querySelectorAll("div." + CLASSES.hitarea)));
            });
            return unique(found);
        };

        if (!settings.prerendered) {
            branches.forEach(function (li) {
                if (hasHiddenList(li)) {
                    // handle closed ones
                    li.classList.add(CLASSES.expandable);
                    replaceClass(li, CLASSES.last, CLASSES.lastExpandable);
                } else {
                    // handle open ones
                    li.classList.add(CLASSES.collapsable);
                    replaceClass(li, CLASSES.last, CLASSES.lastCollapsable);
                }
            });

            // create hitarea if not present
            var hitareas = findHitareas();
            if (!hitareas.length) {
                branches.forEach(function (li) {
                    var div = document.createElement("div");
                    div.className = CLASSES.hitarea;
                    li.insertBefore(div, li.firstChild);
                });
                hitareas = findHitareas();
            }

            hitareas.forEach(function (div) {
                var classes = [CLASSES.hitarea];
                (div.parentElement.getAttribute("class") || "").split(/\s+/).forEach(function (name) {
                    if (name) {
                        classes.push(name + "-hitarea");
                    }
                });
                div.className = classes.join(" ");
            });
        }

        // apply event to hitarea
        findHitareas().forEach(function (div) {
            if (div._treeviewClick) {
                div.removeEventListener("click", div._treeviewClick);
            }
            div._treeviewClick = function () {
                toggler([div]);
            };
            div.addEventListener("click", div._treeviewClick);
        });
    }

    function init(target, options) {
        var roots = resolve(target);
        if (!roots.length) {
            return roots;
        }

        var settings = {};
        var key;
        for (key in options || {}) {
            if (Object.prototype.hasOwnProperty.call(options, key)) {
                settings[key] = options[key];
            }
        }
        if (!settings.cookieId) {
            settings.cookieId = "treeview";
        }
        if (settings.animated === "false") {
            settings.animated = false;
        }

        // user callback gets the <li> as "this"
        if (settings.toggle) {
            var userCallback = settings.toggle;
            settings.toggle = function () {
                return userCallback.apply(this.parentElement, arguments);
            };
        }

        // handle toggle event, elements are children of the <li> to toggle (hitarea, or ul next to a span)
        function toggler(elements) {
            var items = unique(elements.map(function (el) {
                return el.parentElement;
            }).filter(Boolean));

            items.forEach(function (li) {
                // swap classes for hitarea
                childHitareas(li).forEach(function (div) {
                    swapClass(div, CLASSES.collapsableHitarea, CLASSES.expandableHitarea);
                    swapClass(div, CLASSES.lastCollapsableHitarea, CLASSES.lastExpandableHitarea);
                });
                // swap classes for parent li
                swapClass(li, CLASSES.collapsable, CLASSES.expandable);
                swapClass(li, CLASSES.lastCollapsable, CLASSES.lastExpandable);
                // toggle child lists
                childLists(li).forEach(function (ul) {
                    heightToggle(ul, settings.animated, settings.toggle);
                });
            });

            if (settings.unique) {
                var siblings = [];
                items.forEach(function (li) {
                    siblings.push.apply(siblings, siblingsOf(li));
                });
                unique(siblings).forEach(function (li) {
                    childHitareas(li).forEach(function (div) {
                        replaceClass(div, CLASSES.collapsableHitarea, CLASSES.expandableHitarea);
                        replaceClass(div, CLASSES.lastCollapsableHitarea, CLASSES.lastExpandableHitarea);
                    });
                    replaceClass(li, CLASSES.collapsable, CLASSES.expandable);
                    replaceClass(li, CLASSES.lastCollapsable, CLASSES.lastExpandable);
                    childLists(li).forEach(function (ul) {
                        heightHide(ul, settings.animated, settings.toggle);
                    });
                });
            }
        }

        // factory for treecontroller
        function treeController(control) {
            var links = control.querySelectorAll("a");

            // factory for click handlers
            function handler(filter) {
                return function (event) {
                    event.preventDefault();
                    event.stopPropagation();
                    var hitareas = [];
                    roots.forEach(function (root) {
                        hitareas.push.apply(hitareas, toArray(root.querySelectorAll("div." + CLASSES.hitarea)));
                    });
                    // for plain toggle, no filter is provided, otherwise we need to check the parent element
                    toggler(unique(hitareas).filter(function (div) {
                        return !filter || (div.parentElement && div.parentElement.classList.contains(filter));
                    }));
                };
            }

            // first link collapses, second expands, third toggles the tree
            if (links[0]) links[0].addEventListener("click", handler(CLASSES.collapsable));
            if (links[1]) links[1].addEventListener("click", handler(CLASSES.expandable));
            if (links[2]) links[2].addEventListener("click", handler());
        }

        function serialize() {
            var data = branches.map(function (li) {
                return hasVisibleList(li) ? 1 : 0;
            });
            cookie(settings.cookieId, data.join(""), settings.cookieOptions);
        }

        function deserialize() {
            var stored = cookie(settings.cookieId);
            if (stored) {
                var data = stored.split("");
                branches.forEach(function (li, i) {
                    var visible = parseInt(data[i], 10);
                    childLists(li).forEach(visible ? show : hide);
                });
            }
        }

        // add treeview class to activate styles
        roots.forEach(function (root) {
            root.classList.add("treeview");
            root.treeviewToggler = toggler;
        });

        // prepare branches and find all tree items with child lists
        var allItems = [];
        roots.forEach(function (root) {
            allItems.push.apply(allItems, toArray(root.querySelectorAll("li")));
        });
        var branches = prepareBranches(unique(allItems), settings);

        switch (settings.persist) {
            case "cookie":
                var toggleCallback = settings.toggle;
                settings.toggle = function () {
                    serialize();
                    if (toggleCallback) {
                        toggleCallback.apply(this, arguments);
                    }
                };
                deserialize();
                break;

            case "location":
                var href = window.location.href.toLowerCase();
                roots.forEach(function (root) {
                    toArray(root.querySelectorAll("a")).forEach(function (link) {
                        if (!link.href || link.href.toLowerCase() !== href) {
                            return;
                        }
                        link.classList.add("selected");

                        // show all ancestor lists/items up to the tree root, plus the element following the link
                        var items = [];
                        for (var el = link.parentElement; el; el = el.parentElement) {
                            if (el.tagName === "UL" || el.tagName === "LI") {
                                items.push(el);
                            }
                            if (el === root) {
                                break;
                            }
                        }
                        if (link.nextElementSibling) {
                            items.push(link.nextElementSibling);
                        }
                        items.forEach(show);

                        if (settings.prerendered) {
                            // if prerendered is on, replicate the basic class swapping
                            items.forEach(function (item) {
                                if (item.tagName !== "LI") {
                                    return;
                                }
                                swapClass(item, CLASSES.collapsable, CLASSES.expandable);
                                swapClass(item, CLASSES.lastCollapsable, CLASSES.lastExpandable);
                                childHitareas(item).forEach(function (div) {
                                    swapClass(div, CLASSES.collapsableHitarea, CLASSES.expandableHitarea);
                                    swapClass(div, CLASSES.lastCollapsableHitarea, CLASSES.lastExpandableHitarea);
                                });
                            });
                        }
                    });
                });
                break;
        }

        applyClasses(branches, settings, toggler);

        // if control option is set, create the treecontroller and show it
        if (settings.control) {
            resolve(settings.control).forEach(function (control) {
                treeController(control);
                show(control);
            });
        }

        return roots;
    }

    /**
     * Initializes the tree(s). If the DOM is still loading, initialization is deferred until it is ready.
     */
    function initWhenReady(target, options) {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", function () {
                init(target, options);
            });
            return;
        }
        init(target, options);
    }

    window.TreeView = {
        init: initWhenReady,
        cookie: cookie,
        classes: CLASSES
    };

    // optional bridge for existing code calling jQuery(...).treeview(...)
    if (window.jQuery && window.jQuery.fn && !window.jQuery.fn.treeview) {
        window.jQuery.fn.treeview = function (options) {
            init(this.get(), options);
            return this;
        };
    }
})(window, document);
