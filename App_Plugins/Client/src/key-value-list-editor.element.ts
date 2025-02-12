import { css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import type { UUIInputEvent, UUIInputElement } from '@umbraco-cms/backoffice/external/uui';
import { UmbSorterController, UmbSorterConfig } from '@umbraco-cms/backoffice/sorter';
import { repeat } from '@umbraco-cms/backoffice/external/lit';

interface KeyValueItem {
    key: string;
    value: string;
    isDefault?: boolean;
}

@customElement('key-value-list-editor')
export class KeyValueListEditorElement extends UmbLitElement implements UmbPropertyEditorUiElement {
    @property({ type: Array })
    value: Array<KeyValueItem> = [];

    @state()
    private _items: Array<KeyValueItem> = [];

    private static generateUniqueId(item: KeyValueItem): string {
        // Remove spaces and special characters, convert to lowercase
        const key = item.key.toLowerCase().replace(/[^a-z0-9]/g, '');
        const value = item.value.toLowerCase().replace(/[^a-z0-9]/g, '');
        return `${key}_${value}`;
    }

    // Define sorter config separately
    private static SORTER_CONFIG: UmbSorterConfig<KeyValueItem, HTMLElement> = {
        itemSelector: '.item-container',
        containerSelector: '.items-container',
        getUniqueOfElement: (element) => {
            const id = element.getAttribute('data-item-id');
            console.log('getUniqueOfElement', id);
            return id || '';
        },
        getUniqueOfModel: (item) => {
            const id = KeyValueListEditorElement.generateUniqueId(item);
            console.log('getUniqueOfModel', id);
            return id;
        }
    };

    override createRenderRoot() {
        const root = this.attachShadow({
            mode: 'open',
            delegatesFocus: true
        });
        // Ensure styles are adopted
        const sheet = new CSSStyleSheet();
        sheet.replaceSync(KeyValueListEditorElement.styles[0].cssText);
        root.adoptedStyleSheets = [sheet];
        return root;
    }

    // Initialize sorter with config
    #sorter = new UmbSorterController<KeyValueItem, HTMLElement>(this, {
        ...KeyValueListEditorElement.SORTER_CONFIG,
        onChange: ({ model }) => {
            const oldValue = this._items;
            this._items = [...model];
            this.requestUpdate('_items', oldValue);
            this.dispatchChange();
        },
    });

    @property({ type: Boolean, reflect: true })
    public get readonly() {
        return this.#readonly;
    }
    public set readonly(value: boolean) {
        this.#readonly = value;
        if (this.#readonly) {
            this.#sorter.disable();
        } else {
            this.#sorter.enable();
        }
    }
    #readonly = false;

    constructor() {
        super();
    }

    connectedCallback() {
        super.connectedCallback();
        if (this.value && this.value.length > 0) {
            this._items = [...this.value];
            this.#sorter.setModel(this._items);
        }
    }

    willUpdate(changedProperties: Map<string, any>) {
        super.willUpdate(changedProperties);

        if (changedProperties.has('value') && this.value) {
            this._items = [...this.value];
            this.#sorter.setModel(this._items);
        }
    }

    #addItem() {
        const newItems = [
            ...this._items,
            { key: '', value: '', isDefault: false }
        ];
        this._items = newItems;
        this.#sorter.setModel(newItems);
        this.dispatchChange();
        this.#focusNewItem();
    }

    async #focusNewItem() {
        await this.updateComplete;
        const items = this.shadowRoot?.querySelectorAll<UUIInputElement>('uui-input[label="Key"]');
        if (items) {
            const lastItem = items[items.length - 1];
            if (lastItem) {
                console.log('Focusing new item key input');
                lastItem.focus();
                // Optionally, we might need to select the input text
                lastItem.select?.();
            }
        }
    }

    #removeItem(index: number) {
        const newItems = this._items.filter((_, i) => i !== index);
        this._items = newItems;
        this.#sorter.setModel(newItems);
        this.dispatchChange();
    }

    #updateItem(index: number, field: 'key' | 'value', newValue: string) {
        // Create new array with the same references except for the updated item
        this._items = this._items.map((item, i) =>
            i === index
                ? { ...item, [field]: newValue }
                : item
        );

        this.requestUpdate();
        this.dispatchChange();
    }

    #setDefault(index: number) {
        this._items = this._items.map((item, i) => ({
            ...item,
            isDefault: i === index ? (item.isDefault ? false : true) : false
        }));
        this.dispatchChange();
    }

    private dispatchChange() {
        this.value = this._items;
        this.dispatchEvent(new UmbPropertyValueChangeEvent());
    }

    render() {
        return html`
            <div class="flex flex-col">
                <div class="items-container">
                    ${repeat(
            this._items,
            (item) => KeyValueListEditorElement.generateUniqueId(item),
            (item, index) => {
                const itemId = KeyValueListEditorElement.generateUniqueId(item);
                return html`
                                <div 
                                    class="item-container"
                                    id="${itemId}"
                                    data-item-id="${itemId}"
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
                                        .value=${item.key}
                                        placeholder="Enter key"
                                        @change=${(e: UUIInputEvent) =>
                        this.#updateItem(index, 'key', e.target.value as string)}>
                                    </uui-input>
                                    
                                    <uui-input
                                        label="Value"
                                        .value=${item.value}
                                        placeholder="Enter value"
                                        @change=${(e: UUIInputEvent) =>
                        this.#updateItem(index, 'value', e.target.value as string)}>
                                    </uui-input>

                                    <uui-button
                                        label="Set as default"
                                        look=${item.isDefault ? 'primary' : 'default'}
                                        color=${item.isDefault ? 'positive' : 'default'}
                                        @click=${() => this.#setDefault(index)}>
                                        ${item.isDefault ? 'Default' : 'Set Default'}
                                    </uui-button>

                                    <uui-button
                                        label="Remove"
                                        color="danger"
                                        @click=${() => this.#removeItem(index)}>
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
                        @click=${this.#addItem}>
                        Add Item
                    </uui-button>
                </div>
            </div>
        `;
    }

    static styles = [
        css`
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
        `,
    ];
}

export default KeyValueListEditorElement;

declare global {
    interface HTMLElementTagNameMap {
        'key-value-list-editor': KeyValueListEditorElement;
    }
} 