# 🚀 Online Store - MVC Project

## 📌 Description
Online Store is an **ASP.NET Core MVC** e-commerce project developed in **Visual Studio**, designed to manage products, categories, and orders efficiently. The application enables users to browse products, add them to their cart, leave reviews, and receive notifications for price drops on favorite items.

---

## 🎯 Features
### 🛍️ Regular User
- 🏆 **Homepage Highlights**: Displays the **best-selling products** for each category and recently viewed products.
- 🔍 **Search Functionality**: Users can search for products by name.
- 🛒 **Shopping Cart**: Users can add products to their cart and place orders.
- ❤️ **Favorites & Notifications**: Users can mark products as favorites and receive notifications when their price drops. Additionally, an **email notification** is sent to the user when the price of a favorite product decreases.
- 📜 **Recently Viewed Products**: A dedicated section displays the last-viewed items.
- ⭐ **Product Reviews**: Users can leave reviews for purchased products and view their previous orders.

### 🔧 Admin Panel
- 🏗️ **Product Management**: Add, edit, and delete products.
- 📂 **Category & Subcategory Management**: Admins can create categories and assign subcategories.
- 🎨 **Category-Specific Attributes**: Each category can have custom attributes defined by the admin.
- 🏷️ **Category Hierarchy**: Subcategories inherit the attributes of their parent categories but can also have additional specific attributes.

---

## 📸 Screenshots
Below are screenshots illustrating key functionalities:
1. **🔔 Notifications for price drops on favorite products**
   ![Notifications](Screenshot (74).png)
2. **🏠 Homepage with best-selling products and recently viewed items**
   ![Homepage](Screenshot (71).png)
3. **📄 Product details page** (price, reviews, specifications, add to favorites)
   ![Product Actions](Screenshot (72).png)
   ![Product Details&Reviews](Screenshot (73).png)
4. **⚙️ Category and attribute management**
   ![Category Management](Screenshot (75).png)
   ![Subcategory Management](Screenshot(76).png)

---

## 🛠️ Installation and Setup
### 🔧 Steps to Run the Project
1. **📥 Clone the repository**:
   ```bash
   git clone https://github.com/username/OnlineStore-MVC.git
   cd OnlineStore-MVC
   ```
2. **🖥️ Open the project in Visual Studio**
3. **📌 Install necessary dependencies using NuGet Package Manager:**
   - `Microsoft.EntityFrameworkCore.Tools`
   - `EntityFrameworkCore`
   - `EntityFrameworkCore.SqlServer`
4. **📂 Run database migrations:**
   ```bash
   add-migration InitialCreate
   update-database
   ```
   If the migration fails, ensure the `Migrations` folder does not exist, delete it and retry the commands.
5. **📂 Alternative: If you already have a database and just want to import it:**
   - Open **SQL Server LocalDB**
   - Right-click on **Databases** → Select **Publish Data-Tier Application**
   - Import your existing database
6. **🔑 Set up database connection string:**
   - Once the database is created, right-click on the database → Select **Properties**
   - Copy the **Connection String**
   - Paste it in `appsettings.json` under the `"MagazinOnline1"` key:
     ```json
     "MagazinOnline1": "your_connection_string_here"
     ```
7. **▶️ Run the application** in Visual Studio or via the command line:
   ```bash
   dotnet run
   ```
8. **🌍 Access the application in your browser**:
   ```
   http://localhost:7210
   ```

---

## ⚙️ Technologies Used
- 🏗️ **ASP.NET Core MVC** – Backend framework
- 🗄️ **Entity Framework Core** – ORM for database management
- 💾 **SQL Server** – Data storage solutions


