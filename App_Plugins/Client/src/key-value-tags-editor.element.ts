import { css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { UmbSorterController, UmbSorterConfig } from '@umbraco-cms/backoffice/sorter';
import { repeat } from '@umbraco-cms/backoffice/external/lit';
import type { UmbTagsInputElement } from '@umbraco-cms/backoffice/tags';
import '@umbraco-cms/backoffice/tags';

interface KeyValueTagItem {
    title: string;
    key: string;
    tags: Array<string>;
}

@customElement('key-value-tags-editor')
export class KeyValueTagsEditorElement extends UmbLitElement implements UmbPropertyEditorUiElement {
    @property({ type: Array })
    public value?: Array<KeyValueTagItem>;

    @property({ type: Object })
    public config: any;

    @state()
    private _items: Array<KeyValueTagItem> = [];

    private static generateUniqueId(item: KeyValueTagItem): string {
        const title = item.title.toLowerCase().replace(/[^a-z0-9]/g, '');
        const key = item.key.toLowerCase().replace(/[^a-z0-9]/g, '');
        return `${title}_${key}`;
    }

    private static SORTER_CONFIG: UmbSorterConfig<KeyValueTagItem, HTMLElement> = {
        itemSelector: '.key-value-tags__item',
        containerSelector: '.key-value-tags',
        getUniqueOfElement: (element) => {
            const id = element.getAttribute('data-item-id');
            return id || '';
        },
        getUniqueOfModel: (item) => {
            return KeyValueTagsEditorElement.generateUniqueId(item);
        }
    };

    #sorter = new UmbSorterController<KeyValueTagItem, HTMLElement>(this, {
        ...KeyValueTagsEditorElement.SORTER_CONFIG,
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
        this.value = [];
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

    private dispatchChange() {
        this.value = this._items;
        this.dispatchEvent(new UmbPropertyValueChangeEvent());
    }

    render() {
        return html`
            <div class="key-value-tags">
                ${repeat(
            this._items,
            (item) => KeyValueTagsEditorElement.generateUniqueId(item),
            (item, index) => {
                const itemId = KeyValueTagsEditorElement.generateUniqueId(item);
                return html`
                            <div 
                                class="key-value-tags__item"
                                id="${itemId}"
                                data-item-id="${itemId}"
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
                                            .value=${item.title}
                                            placeholder="Enter title"
                                            ?readonly=${this.readonly}
                                            @change=${(e: Event) => this.#onTitleChange(index, e)}>
                                        </uui-input>

                                        <uui-input
                                            .value=${item.key}
                                            placeholder="Enter key"
                                            ?readonly=${this.readonly}
                                            @change=${(e: Event) => this.#onKeyChange(index, e)}>
                                        </uui-input>
                                    </div>

                                    <umb-tags-input
                                        .value=${item.tags.join(',')}
                                        .items=${item.tags}
                                        .config=${this.config}
                                        ?readonly=${this.readonly}
                                        @change=${(e: CustomEvent) => this.#onTagsChange(index, e)}>
                                    </umb-tags-input>
                                </div>

                                ${!this.readonly ? html`
                                    <uui-button
                                        compact
                                        look="secondary"
                                        color="danger"
                                        label="Remove"
                                        @click=${() => this.#onRemove(index)}>
                                        <uui-icon name="icon-trash"></uui-icon>
                                    </uui-button>
                                ` : ''}
                            </div>
                        `;
            }
        )}

                ${!this.readonly ? html`
                    <div class="key-value-tags__item">
                        <div></div>
                        <div class="add-item-container">
                            <uui-button
                                look="primary"
                                label="Add item"
                                @click=${this.#onAdd}>
                                <uui-icon name="add"></uui-icon>
                                Add item
                            </uui-button>
                        </div>
                        <div></div>
                    </div>
                ` : ''}
            </div>
        `;
    }

    static styles = [
        css`
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

    #onAdd() {
        const newItems = [
            ...this._items,
            { title: '', key: '', tags: [] }
        ];
        this._items = newItems;
        this.#sorter.setModel(newItems);
        this.dispatchChange();
    }

    #onRemove(index: number) {
        const newItems = this._items.filter((_, i) => i !== index);
        this._items = newItems;
        this.#sorter.setModel(newItems);
        this.dispatchChange();
    }

    #onTitleChange(index: number, event: Event) {
        const input = event.target as HTMLInputElement;
        this._items = this._items.map((item, i) =>
            i === index ? { ...item, title: input.value } : item
        );
        this.dispatchChange();
    }

    #onKeyChange(index: number, event: Event) {
        const input = event.target as HTMLInputElement;
        this._items = this._items.map((item, i) =>
            i === index ? { ...item, key: input.value } : item
        );
        this.dispatchChange();
    }

    #onTagsChange(index: number, event: CustomEvent) {
        const tags = ((event.target as UmbTagsInputElement).value as string)
            .split(',')
            .filter(tag => tag.trim() !== '');

        this._items = this._items.map((item, i) =>
            i === index ? { ...item, tags } : item
        );
        this.dispatchChange();
    }
}

export default KeyValueTagsEditorElement;

declare global {
    interface HTMLElementTagNameMap {
        'key-value-tags-editor': KeyValueTagsEditorElement;
    }
} 