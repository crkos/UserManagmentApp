export default class TableContainer {
    _tableRef;
    _tableRows;
    _userSelectors;
    _currentlySelected = [];
    _selectAllIcon;

    /**
     * Constructor
     * Get all the necessary data.
     * 
     * @param {string} tableId
     * @param {string} selectAllIconId
     */
    constructor(tableId, selectAllIconId) {
        this._tableRef = $(tableId);
        this._tableRows = this._tableRef.find("tr").filter((_, element) => {
            const $tr = $(element)
            return !$tr.hasClass("header")
        })
        this._userSelectors = this._tableRows.find("input[type='checkbox']").toArray();
        this._selectAllIcon = $(selectAllIconId);

        this.select = this.select.bind(this);
        this.selectAll = this.selectAll.bind(this);
        this._userSelectors.forEach((el) => {
            $(el).click(this.select)
        })
        this._selectAllIcon.click(this.selectAll)
    }

    /**
    * Selects or deselects all of the checkboxes depending of the current state.
    */
    selectAll() {
        const noneSelected = this._currentlySelected.length === 0;

        if (noneSelected) {
            this._userSelectors.forEach((el) => {
                el.checked = true;
                this.addToSelection(el);
            });
        } else {
            this._userSelectors.forEach((el) => {
                el.checked = false;
                this.removeFromSelection(el);
            });
        }

        this.updateSelectAllIcon();
    }

    /**
     * Select and add a row, deselects and removes if selected already.
     * @param {HTMLInputElement} element 
     */
    select(element) {
        const checkbox = element.target;
        const isSelected = checkbox.checked;

        if (isSelected) {
            this.addToSelection(checkbox);
        } else {
            this.removeFromSelection(checkbox);
        }
        this.updateSelectAllIcon();
    }

    addToSelection(checkbox) {
        if (!this._currentlySelected.includes(checkbox)) {
            this._currentlySelected.push(checkbox);
        }
    }

    removeFromSelection(checkbox) {
        const index = this._currentlySelected.indexOf(checkbox);

        if (index !== -1) {
            this._currentlySelected.splice(index, 1);
        }
    }

    updateSelectAllIcon() {
        const totalCheckboxes = this._userSelectors.length;
        const checkedCount = this._currentlySelected.length;


        if (checkedCount === 0) {
            this._selectAllIcon.removeClass('bi-dash-square').addClass('bi-plus-square');
        } else if (checkedCount > 0) {
            this._selectAllIcon.removeClass('bi-plus-square').addClass('bi-dash-square');
        }
    }

    /**
     * 
     * @returns {number[]}
     */
    getValues() {
        return this._currentlySelected.map((val) => Number($(val).val()))
    }
}