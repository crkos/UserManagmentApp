import TableContainer from "./TableContainer.js";

export default class Actions {
    /** @type {TableContainer} */
    _tableInstance;
    _blockBtn;
    _deleteBtn;
    _unblockBtn;

    /**
     * Constructor
     * @param {TableContainer} tableContainer - Instance of TableContainer
     * @param {string} blockBtnId
     * @param {string} deleteBtnId
     * @param {string} unblockBtnId
     */
    constructor(tableContainer, blockBtnId, deleteBtnId, unblockBtnId) {
        this._tableInstance = tableContainer
        this._blockBtn = $(blockBtnId)
        this._deleteBtn = $(deleteBtnId)
        this._unblockBtn = $(unblockBtnId)

        this.setStatus = this.setStatus.bind(this);
        this.deleteUsers = this.deleteUsers.bind(this);

        this._blockBtn.click(this.setStatus);
        this._unblockBtn.click(this.setStatus);
        this._deleteBtn.click(this.deleteUsers);
    }

    /**
     * 
     *
     * @param {SubmitEvent} action
     * @returns
     */
    setStatus(action) {
        const userIds = this._tableInstance.getValues();

        const button = $(action.currentTarget);
        const status = button.attr('value');


        if (userIds.length === 0) {
            swal({
                title: "Error",
                text: "No users were selected",
                icon: "warning",
                button: "OK"
            })
            return
        }

        const data = {
            userIds,
            status
        }


        $.ajax({
            url: `/UserChange/SetStatus`,
            type: "PATCH",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (response) {
                swal({
                    title: "Success",
                    text: response.message,
                    icon: "success",
                    button: "OK"
                }).then((_) => {
                    location.reload();
                });
            },
            error: function (xhr, status, error) {
                swal({
                    title: "Error",
                    text: `An error occured while unblocking users: ${error}`,
                    icon: "error",
                    button: "OK"
                })
            }
        })
    }

    deleteUsers() {
        const userIds = this._tableInstance.getValues();

        if (userIds.length === 0) {
            swal({
                title: "Error",
                text: "No users were selected",
                icon: "warning",
                button: "OK"
            })
            return
        }

        $.ajax({
            url: `/UserChange/Delete`,
            type: "DELETE",
            contentType: "application/json",
            data: JSON.stringify(userIds),
            success: function (response) {
                swal({
                    title: "Success",
                    text: response.message,
                    icon: "success",
                    button: "OK"
                }).then((_) => {
                    location.reload();
                });
            },
            error: function (xhr, status, error) {
                swal({
                    title: "Error",
                    text: `An error occured while deleting users: ${error}`,
                    icon: "error",
                    button: "OK"
                })
            }
        })
    }

}