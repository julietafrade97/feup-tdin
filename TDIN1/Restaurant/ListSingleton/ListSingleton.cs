﻿using Common;
using System;
using System.Collections.Generic;
using System.Threading;

public class ListSingleton : MarshalByRefObject, IListSingleton {
    List <Order> orders;
    List <Table> tables;
    List <Product> products;

    public event AlterDelegate alterEvent;
    int type = 1;
    public ListSingleton()
    {
        Console.WriteLine("Constructor called.");

        //TABLES
        tables = new List<Table>();
        tables.Add(new Table(0, Table.State.CLOSED));
        tables.Add(new Table(1, Table.State.CLOSED));
        tables.Add(new Table(2, Table.State.CLOSED));
        tables.Add(new Table(3, Table.State.CLOSED));
        tables.Add(new Table(4, Table.State.CLOSED));
        tables.Add(new Table(5, Table.State.CLOSED));

        //PRODUCTS
        products = new List<Product>();
        products.Add(new Product(0, "Cocacola", 1.5F, Product.Type.DRINK));
        products.Add(new Product(1, "Icetea", 2.5F, Product.Type.DRINK));
        products.Add(new Product(2, "Water", 3.5F, Product.Type.DRINK));
        products.Add(new Product(3, "Croissant", 4.5F, Product.Type.FOOD));
        products.Add(new Product(4, "Hamburguer", 5.5F, Product.Type.FOOD));
        products.Add(new Product(5, "Fish", 6.5F, Product.Type.FOOD));

        //ORDERS
        orders = new List<Order>();
        /*Order order0 = new Order(9, products[0], 2, Order.State.NOT_PROCESSED); //drink
        Order order1 = new Order(8, products[1], 4, Order.State.PROCESSING); //drink
        Order order2 = new Order(7, products[2], 7, Order.State.FINISHED); //drink
        Order order4 = new Order(2, products[5], 10, Order.State.FINISHED); //food
        Order order5 = new Order(9, products[4], 2, Order.State.NOT_PROCESSED); //food
        Order order6 = new Order(8, products[5], 4, Order.State.PROCESSING); //food
        orders.Add(order0);
        orders.Add(order1);
        orders.Add(order2);
        orders.Add(order4);
        orders.Add(order5);
        orders.Add(order6);*/
    }

    public override object InitializeLifetimeService()
    {
        return null;
    }

    public List<Table> getTables(Table.State state)
    {
        Console.WriteLine("getTables() called.");
        List<Table> res = new List<Table>();
        foreach (Table table in tables)
        {
            if (table.StateProperty.Equals(state))
                res.Add(table);
        }
        return res;
    }

    public List<Table> getTables()
    {
        Console.WriteLine("getTables() called.");
        List<Table> res = new List<Table>();
        foreach (Table table in tables)
        {
                res.Add(table);
        }
        return res;
    }

    public List<Product> getProducts()
    {
        Console.WriteLine("getProducts() called.");
        return products;
    }

    public List<Order> getOrders(Order.State state)
    {
        Console.WriteLine("getOrders(state) called.");
        List<Order> res = new List<Order>();
        foreach(Order order in orders)
        {
            if (order.StateProperty.Equals(state))
                res.Add(order);
        }
        return res;
    }

    public List<Order> getOrdersByType(Order.State state, Product.Type type)
    {
        Console.WriteLine("getOrdersByType(state,type) called.");
        List<Order> res = new List<Order>();
        foreach (Order order in orders)
        {
           if (order.StateProperty.Equals(state) && type.Equals(order.Product.TypeProperty))
            {
                res.Add(order);
            }
                
        }
        return res;
    }

    public List<Order> getOrdersByTable(int tableId)
    {
        Console.WriteLine("getOrdersByTable(state,tableId) called.");
        List<Order> res = new List<Order>();
        foreach (Order order in orders)
        {
            if (order.TableId == tableId)
            {
                res.Add(order);
            }

        }
        return res;
    }

    public List<Order> getOrdersByTable(int tableId, Order.State state)
    {
        Console.WriteLine("getOrdersByTable(state,tableId) called.");
        List<Order> res = new List<Order>();
        foreach (Order order in orders)
        {
            if (order.TableId == tableId && order.StateProperty.Equals(state))
            {
                res.Add(order);
            }

        }
        return res;
    }

    public void addOrder(Order order)
    {
        orders.Add(order);
        NotifyClients(Operation.Added_Order, order, 0);

        Table table = tables.Find(t => t.Id.Equals(order.TableId));
        if (!table.StateProperty.Equals(Table.State.WAITING))
        {
            table.StateProperty = Table.State.WAITING;
            NotifyClients(Operation.Changed_Table_State, null, 0);
        }
    }

    public void changeOrderStatus(Guid orderId, Order.State newStatus)
    {
        Order norder = null;
        bool istableDone = true;
        int tableId = 0;

        foreach (Order it in orders)
        {
            if (it.Id == orderId)
            {
                it.StateProperty = newStatus;
                norder = it;
                tableId = norder.TableId;
                break;
            }

        }
        NotifyClients(Operation.Changed_Order_State, norder, tableId);

        foreach (Order it in orders)
        {
            if (it.TableId == tableId && !it.StateProperty.Equals(Order.State.DELIVERED) && !it.StateProperty.Equals(Order.State.CLOSED))
            {
                istableDone = false;
                break;
            }
        }

        if(istableDone == true)
        {
            Table table = tables.Find(t => t.Id.Equals(tableId));
            if (table != null && !table.StateProperty.Equals(Table.State.DONE))
            {
                table.StateProperty = Table.State.DONE;
                NotifyClients(Operation.Changed_Table_State, null, tableId);
            }
        }
        else if (istableDone != true)
        {
            Table table = tables.Find(t => t.Id.Equals(tableId));
            if (table!= null && !table.StateProperty.Equals(Table.State.WAITING))
            {
                table.StateProperty = Table.State.WAITING;
                NotifyClients(Operation.Changed_Table_State, null, tableId);
            }
        }
    }

    public void changeTableStatus(int tableId, Table.State newStatus)
    {
        Table table = tables.Find(t => t.Id.Equals(tableId));
        table.StateProperty = newStatus;
        NotifyClients(Operation.Changed_Table_State, null, tableId);

    }

    void NotifyClients(Operation op, Order order, int tableId)
    {
        if (alterEvent != null)
        {
            Delegate[] invkList = alterEvent.GetInvocationList();

            foreach (AlterDelegate handler in invkList)
            {
                new Thread(() => {
                    try
                    {
                        handler(op, order, tableId);
                        Console.WriteLine("Invoking event handler");
                    }
                    catch (Exception)
                    {
                        alterEvent -= handler;
                        Console.WriteLine("Exception: Removed an event handler");
                    }
                }).Start();
            }
        }
    }
}

