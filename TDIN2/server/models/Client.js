const Sequelize = require('sequelize');
const db = require ('../config/database');

const Client = db.define('Client', {
    name: {
      type: Sequelize.STRING
    }
  }, {
      // disable the modification of table names; By default, sequelize will automatically
      // transform all passed model names (first parameter of define) into plural.
      // if you don't want that, set the following
      freezeTableName: true,
    });

module.exports = Client;