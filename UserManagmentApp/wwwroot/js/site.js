import TableContainer from "./TableContainer.js";
import Actions from "./Actions.js";
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const table = new TableContainer("#user-data-table", "#select-all-icon");

const actions = new Actions(table, "#block-users-btn", "#delete-users-btn", "#unblock-users-btn");