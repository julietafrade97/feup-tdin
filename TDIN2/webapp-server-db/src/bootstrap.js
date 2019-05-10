module.exports = async () => {

    const Client = require ("./models/Client");
    const Order = require ("./models/Order");
    const Book = require ("./models/Book");
    const uuidv1 = require('uuid/v1');

    Client.hasMany(Order, {as: "Orders", foreignKey: 'clientId'});
    Order.belongsTo(Client, { as: "Client", foreignKey: 'clientId'});

    Order.hasMany(Book, {as: "Books", foreignKey: 'bookId'});
    Book.belongsTo(Order, { as: "Order", foreignKey: 'bookId'});

    const errHandler = (err) => {
        console.error("Error: ", err);
    }

    const client = await Client.create({ 
        name: "Bernardo", 
        email: "up201404464@fe.up.pt",
        address: "Rua da Feup",
    }).catch(errHandler);

    const book = await Book.create({ 
        title: "IT - A COISA :)",
        stock: 3,
        price: 19.99,
    }).catch(errHandler);

    const order = await Order.create({ 
        id: uuidv1(),
        clientId: client.id,
        bookId: book.id,
        quantity: 2,
        totalPrice: 13.4,
        //dispatchedDate: Sequelize.DATE,
        state: "waiting",
    }).catch(errHandler);



    const clients = await Client.findAll({ where: { name: 'Bernardo'}, include: [{model: Order, as: "Orders"}]}).catch(errHandler);


    //console.log(clients[0].name);
}