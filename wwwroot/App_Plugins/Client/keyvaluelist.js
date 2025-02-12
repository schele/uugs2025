import { css as k, property as b, state as S, customElement as E, html as f, repeat as $ } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as D } from "@umbraco-cms/backoffice/lit-element";
import { UmbPropertyValueChangeEvent as U } from "@umbraco-cms/backoffice/property-editor";
import { UmbSorterController as I } from "@umbraco-cms/backoffice/sorter";
var O = Object.defineProperty, q = Object.getOwnPropertyDescriptor, y = (e) => {
  throw TypeError(e);
}, d = (e, t, i, a) => {
  for (var s = a > 1 ? void 0 : a ? q(t, i) : t, h = e.length - 1, p; h >= 0; h--)
    (p = e[h]) && (s = (a ? p(t, i, s) : p(s)) || s);
  return a && s && O(t, i, s), s;
}, g = (e, t, i) => t.has(e) || y("Cannot " + i), o = (e, t, i) => (g(e, t, "read from private field"), i ? i.call(e) : t.get(e)), m = (e, t, i) => t.has(e) ? y("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), z = (e, t, i, a) => (g(e, t, "write to private field"), t.set(e, i), i), l = (e, t, i) => (g(e, t, "access private method"), i), n, c, u, _, w, x, v, C;
let r = class extends D {
  constructor() {
    super(), m(this, u), this.value = [], this._items = [], m(this, n, new I(this, {
      ...r.SORTER_CONFIG,
      onChange: ({ model: e }) => {
        const t = this._items;
        this._items = [...e], this.requestUpdate("_items", t), this.dispatchChange();
      }
    })), m(this, c, !1);
  }
  static generateUniqueId(e) {
    const t = e.key.toLowerCase().replace(/[^a-z0-9]/g, ""), i = e.value.toLowerCase().replace(/[^a-z0-9]/g, "");
    return `${t}_${i}`;
  }
  createRenderRoot() {
    const e = this.attachShadow({
      mode: "open",
      delegatesFocus: !0
    }), t = new CSSStyleSheet();
    return t.replaceSync(r.styles[0].cssText), e.adoptedStyleSheets = [t], e;
  }
  get readonly() {
    return o(this, c);
  }
  set readonly(e) {
    z(this, c, e), o(this, c) ? o(this, n).disable() : o(this, n).enable();
  }
  connectedCallback() {
    super.connectedCallback(), this.value && this.value.length > 0 && (this._items = [...this.value], o(this, n).setModel(this._items));
  }
  willUpdate(e) {
    super.willUpdate(e), e.has("value") && this.value && (this._items = [...this.value], o(this, n).setModel(this._items));
  }
  dispatchChange() {
    this.value = this._items, this.dispatchEvent(new U());
  }
  render() {
    return f`
            <div class="flex flex-col">
                <div class="items-container">
                    ${$(
      this._items,
      (e) => r.generateUniqueId(e),
      (e, t) => {
        const i = r.generateUniqueId(e);
        return f`
                                <div 
                                    class="item-container"
                                    id="${i}"
                                    data-item-id="${i}"
                                    ?disabled=${this.readonly}>
                                    <uui-button 
                                        class="drag-handle"
                                        label="Drag to reorder"
                                        tabindex="-1"
                                        compact>
                                        <uui-icon name="icon-navigation"></uui-icon>
                                    </uui-button>

                                    <uui-input
                                        label="Key"
                                        .value=${e.key}
                                        placeholder="Enter key"
                                        @change=${(a) => l(this, u, v).call(this, t, "key", a.target.value)}>
                                    </uui-input>
                                    
                                    <uui-input
                                        label="Value"
                                        .value=${e.value}
                                        placeholder="Enter value"
                                        @change=${(a) => l(this, u, v).call(this, t, "value", a.target.value)}>
                                    </uui-input>

                                    <uui-button
                                        label="Set as default"
                                        look=${e.isDefault ? "primary" : "default"}
                                        color=${e.isDefault ? "positive" : "default"}
                                        @click=${() => l(this, u, C).call(this, t)}>
                                        ${e.isDefault ? "Default" : "Set Default"}
                                    </uui-button>

                                    <uui-button
                                        label="Remove"
                                        color="danger"
                                        @click=${() => l(this, u, x).call(this, t)}>
                                        <uui-icon name="icon-trash"></uui-icon>
                                    </uui-button>
                                </div>
                            `;
      }
    )}
                </div>

                <div class="add-item-container">
                    <uui-button
                        label="Add item"
                        look="primary"
                        @click=${l(this, u, _)}>
                        Add Item
                    </uui-button>
                </div>
            </div>
        `;
  }
};
n = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakMap();
u = /* @__PURE__ */ new WeakSet();
_ = function() {
  const e = [
    ...this._items,
    { key: "", value: "", isDefault: !1 }
  ];
  this._items = e, o(this, n).setModel(e), this.dispatchChange(), l(this, u, w).call(this);
};
w = async function() {
  var t, i;
  await this.updateComplete;
  const e = (t = this.shadowRoot) == null ? void 0 : t.querySelectorAll('uui-input[label="Key"]');
  if (e) {
    const a = e[e.length - 1];
    a && (console.log("Focusing new item key input"), a.focus(), (i = a.select) == null || i.call(a));
  }
};
x = function(e) {
  const t = this._items.filter((i, a) => a !== e);
  this._items = t, o(this, n).setModel(t), this.dispatchChange();
};
v = function(e, t, i) {
  this._items = this._items.map(
    (a, s) => s === e ? { ...a, [t]: i } : a
  ), this.requestUpdate(), this.dispatchChange();
};
C = function(e) {
  this._items = this._items.map((t, i) => ({
    ...t,
    isDefault: i === e ? !t.isDefault : !1
  })), this.dispatchChange();
};
r.SORTER_CONFIG = {
  itemSelector: ".item-container",
  containerSelector: ".items-container",
  getUniqueOfElement: (e) => {
    const t = e.getAttribute("data-item-id");
    return console.log("getUniqueOfElement", t), t || "";
  },
  getUniqueOfModel: (e) => {
    const t = r.generateUniqueId(e);
    return console.log("getUniqueOfModel", t), t;
  }
};
r.styles = [
  k`
            :host {
                display: block;
                padding: var(--uui-size-space-2);
            }

            .items-container {
                display: flex;
                flex-direction: column;
                gap: var(--uui-size-space-3);
                margin-bottom: var(--uui-size-space-4);
            }

            .item-container {
                cursor: default;
                background: var(--uui-color-surface);
                border-radius: var(--uui-border-radius);
                padding: var(--uui-size-space-1);
                display: grid;
                grid-template-columns: auto 1fr 1fr 120px auto;
                gap: var(--uui-size-space-3);
                align-items: center;
                transition: all 120ms ease-in-out;
            }

            .item-container[disabled] {
                opacity: 0.7;
                pointer-events: none;
            }

            .drag-handle {
                cursor: move;
                color: var(--uui-color-text-alt);
                transition: all 120ms ease;
                width: 40px;
                display: flex;
                align-items: center;
                justify-content: center;
            }

            .drag-handle:hover {
                color: var(--uui-color-text);
                background: var(--uui-color-surface-emphasis);
                border-color: var(--uui-color-border-emphasis);
            }

            uui-input {
                min-width: 200px;
            }

            /* Draggable styles */
            .draggable-mirror {
                background: var(--uui-color-surface);
                border: 1px solid var(--uui-color-border-emphasis);
                border-radius: var(--uui-border-radius);
                padding: var(--uui-size-space-4);
                box-shadow: var(--uui-shadow-depth-3);
                opacity: 0.9;
            }

            .draggable-source--is-dragging {
                opacity: 0.3;
                border: 1px dashed var(--uui-color-border);
            }

            /* Add item button container */
            .add-item-container {
                margin-top: var(--uui-size-space-2);
            }

            /* Responsive adjustments */
            @media (max-width: 768px) {
                .item-container {
                    grid-template-columns: auto 1fr auto;
                    gap: var(--uui-size-space-2);
                }

                uui-input {
                    min-width: 150px;
                    grid-column: 2 / -1;
                }

                .item-container uui-button:not(.drag-handle) {
                    grid-column: 2 / -1;
                    width: auto;
                }
            }

            /* Common button styles - matching input height */
            .items-container uui-button {
                height: var(--uui-button-height);
                border: 1px solid var(--uui-color-border);
                border-radius: var(--uui-border-radius);
                transition: all 120ms ease;
            }

            /* Drag handle */
            .drag-handle {
                cursor: move;
                color: var(--uui-color-text-alt);
                width: var(--uui-button-height);
                display: flex;
                align-items: center;
                justify-content: center;
            }

            /* Default button */
            uui-button[label="Set as default"] {
                width: 120px;
                text-align: center;
                justify-content: center;
            }

            /* Remove button */
            uui-button[label="Remove"] {
                width: var(--uui-button-height);
            }

            /* Add item button */
            .add-item-container uui-button {
                border-color: var(--uui-color-primary);
                /* padding: 0 var(--uui-size-space-4); */
            }

            .add-item-container uui-button:hover {
                background: var(--uui-color-primary-emphasis);
                border-color: var(--uui-color-primary-emphasis);
                color: var(--uui-color-surface);
            }
        `
];
d([
  b({ type: Array })
], r.prototype, "value", 2);
d([
  S()
], r.prototype, "_items", 2);
d([
  b({ type: Boolean, reflect: !0 })
], r.prototype, "readonly", 1);
r = d([
  E("key-value-list-editor")
], r);
const V = r;
export {
  r as KeyValueListEditorElement,
  V as default
};
//# sourceMappingURL=keyvaluelist.js.map
