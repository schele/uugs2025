import { css as w, property as v, state as $, customElement as E, html as h, repeat as U } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as O } from "@umbraco-cms/backoffice/lit-element";
import { UmbPropertyValueChangeEvent as z } from "@umbraco-cms/backoffice/property-editor";
import { UmbSorterController as I } from "@umbraco-cms/backoffice/sorter";
import "@umbraco-cms/backoffice/tags";
var T = Object.defineProperty, M = Object.getOwnPropertyDescriptor, y = (e) => {
  throw TypeError(e);
}, c = (e, t, i, a) => {
  for (var s = a > 1 ? void 0 : a ? M(t, i) : t, p = e.length - 1, g; p >= 0; p--)
    (g = e[p]) && (s = (a ? g(t, i, s) : g(s)) || s);
  return a && s && T(t, i, s), s;
}, _ = (e, t, i) => t.has(e) || y("Cannot " + i), n = (e, t, i) => (_(e, t, "read from private field"), i ? i.call(e) : t.get(e)), m = (e, t, i) => t.has(e) ? y("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), R = (e, t, i, a) => (_(e, t, "write to private field"), t.set(e, i), i), l = (e, t, i) => (_(e, t, "access private method"), i), o, d, u, f, b, k, x, C;
let r = class extends O {
  constructor() {
    super(), m(this, u), this._items = [], m(this, o, new I(this, {
      ...r.SORTER_CONFIG,
      onChange: ({ model: e }) => {
        const t = this._items;
        this._items = [...e], this.requestUpdate("_items", t), this.dispatchChange();
      }
    })), m(this, d, !1), this.value = [];
  }
  static generateUniqueId(e) {
    const t = e.title.toLowerCase().replace(/[^a-z0-9]/g, ""), i = e.key.toLowerCase().replace(/[^a-z0-9]/g, "");
    return `${t}_${i}`;
  }
  get readonly() {
    return n(this, d);
  }
  set readonly(e) {
    R(this, d, e), n(this, d) ? n(this, o).disable() : n(this, o).enable();
  }
  connectedCallback() {
    super.connectedCallback(), this.value && this.value.length > 0 && (this._items = [...this.value], n(this, o).setModel(this._items));
  }
  willUpdate(e) {
    super.willUpdate(e), e.has("value") && this.value && (this._items = [...this.value], n(this, o).setModel(this._items));
  }
  dispatchChange() {
    this.value = this._items, this.dispatchEvent(new z());
  }
  render() {
    return h`
            <div class="key-value-tags">
                ${U(
      this._items,
      (e) => r.generateUniqueId(e),
      (e, t) => {
        const i = r.generateUniqueId(e);
        return h`
                            <div 
                                class="key-value-tags__item"
                                id="${i}"
                                data-item-id="${i}"
                                ?disabled=${this.readonly}>
                                
                                <uui-button 
                                    class="drag-handle"
                                    label="Drag to reorder"
                                    tabindex="-1"
                                    style="min-height: 40px; min-width: 40px;"
                                    compact>
                                    <uui-icon name="icon-navigation"></uui-icon>
                                </uui-button>

                                <div class="key-value-tags__item-content">
                                    <div class="key-value-tags__item-inputs">
                                        <uui-input
                                            .value=${e.title}
                                            placeholder="Enter title"
                                            ?readonly=${this.readonly}
                                            @change=${(a) => l(this, u, k).call(this, t, a)}>
                                        </uui-input>

                                        <uui-input
                                            .value=${e.key}
                                            placeholder="Enter key"
                                            ?readonly=${this.readonly}
                                            @change=${(a) => l(this, u, x).call(this, t, a)}>
                                        </uui-input>
                                    </div>

                                    <umb-tags-input
                                        .value=${e.tags.join(",")}
                                        .items=${e.tags}
                                        .config=${this.config}
                                        ?readonly=${this.readonly}
                                        @change=${(a) => l(this, u, C).call(this, t, a)}>
                                    </umb-tags-input>
                                </div>

                                ${this.readonly ? "" : h`
                                    <uui-button
                                        compact
                                        look="secondary"
                                        color="danger"
                                        label="Remove"
                                        @click=${() => l(this, u, b).call(this, t)}>
                                        <uui-icon name="icon-trash"></uui-icon>
                                    </uui-button>
                                `}
                            </div>
                        `;
      }
    )}

                ${this.readonly ? "" : h`
                    <div class="key-value-tags__item">
                        <div></div>
                        <div class="add-item-container">
                            <uui-button
                                look="primary"
                                label="Add item"
                                @click=${l(this, u, f)}>
                                <uui-icon name="add"></uui-icon>
                                Add item
                            </uui-button>
                        </div>
                        <div></div>
                    </div>
                `}
            </div>
        `;
  }
};
o = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakMap();
u = /* @__PURE__ */ new WeakSet();
f = function() {
  const e = [
    ...this._items,
    { title: "", key: "", tags: [] }
  ];
  this._items = e, n(this, o).setModel(e), this.dispatchChange();
};
b = function(e) {
  const t = this._items.filter((i, a) => a !== e);
  this._items = t, n(this, o).setModel(t), this.dispatchChange();
};
k = function(e, t) {
  const i = t.target;
  this._items = this._items.map(
    (a, s) => s === e ? { ...a, title: i.value } : a
  ), this.dispatchChange();
};
x = function(e, t) {
  const i = t.target;
  this._items = this._items.map(
    (a, s) => s === e ? { ...a, key: i.value } : a
  ), this.dispatchChange();
};
C = function(e, t) {
  const i = t.target.value.split(",").filter((a) => a.trim() !== "");
  this._items = this._items.map(
    (a, s) => s === e ? { ...a, tags: i } : a
  ), this.dispatchChange();
};
r.SORTER_CONFIG = {
  itemSelector: ".key-value-tags__item",
  containerSelector: ".key-value-tags",
  getUniqueOfElement: (e) => e.getAttribute("data-item-id") || "",
  getUniqueOfModel: (e) => r.generateUniqueId(e)
};
r.styles = [
  w`
            .key-value-tags {
                display: flex;
                flex-direction: column;
                gap: var(--uui-size-space-3);
            }

            .key-value-tags__item {
                display: grid;
                grid-template-columns: 40px 1fr 55.0278px;
                gap: var(--uui-size-space-3);
                background: var(--uui-color-surface);
                border-radius: var(--uui-border-radius);
                padding: var(--uui-size-space-3) 0;
            }

            .key-value-tags__item-content {
                display: flex;
                flex-direction: column;
                gap: var(--uui-size-space-3);
                grid-column: 2;
            }

            .key-value-tags__item-inputs {
                display: flex;
                gap: var(--uui-size-space-3);
                width: 100%;
            }

            .key-value-tags__item-inputs uui-input {
                flex: 1;
                min-height: 40px;
            }

            /* Drag handle - first column */
            .drag-handle {
                grid-column: 1;
                cursor: move;
                color: var(--uui-color-text-alt);
                min-width: 40px;
                min-height: 40px;
                width: 40px;
                height: 40px;
                display: flex;
                align-items: center;
                justify-content: center;
                align-self: start;
            }

            /* Remove button - last column */
            uui-button[label="Remove"] {
                grid-column: 3;
                width: 55.0278px;
                height: 40px;
                min-height: 40px;
                align-self: start;
            }

            /* Tags input */
            umb-tags-input {
                width: 100%;
            }

            /* Common button styles */
            .key-value-tags uui-button {
                height: var(--uui-button-height);
                border: 1px solid var(--uui-color-border);
                border-radius: var(--uui-border-radius);
                transition: all 120ms ease;
            }

            /* Add item button */
            .add-item-container {
                grid-column: 2;
                display: flex;
                align-items: center;
            }

            .add-item-container uui-button {
                border-color: var(--uui-color-primary);
            }

            /* Draggable styles  */
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

            .key-value-tags__item-header {
                display: grid;
                grid-template-columns: auto 1fr auto;
                gap: var(--uui-size-space-3);
                align-items: center;
            }
        `
];
c([
  v({ type: Array })
], r.prototype, "value", 2);
c([
  v({ type: Object })
], r.prototype, "config", 2);
c([
  $()
], r.prototype, "_items", 2);
c([
  v({ type: Boolean, reflect: !0 })
], r.prototype, "readonly", 1);
r = c([
  E("key-value-tags-editor")
], r);
const K = r;
export {
  r as KeyValueTagsEditorElement,
  K as default
};
//# sourceMappingURL=keyvaluetags.js.map
