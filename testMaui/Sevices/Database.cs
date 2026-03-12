using MySql.Data.MySqlClient;
using testMaui.Models;

namespace testMaui.Sevices
{
    public class Database
    {
        private static readonly string connectionString =
            "server=localhost;database=diet_planner;user=root;password=123456;port=3307;";

        // ========== Продукты ==========
        public static List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT id, name, calories, proteins, fats, carbs FROM products", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Calories = reader.GetDouble("calories"),
                    Proteins = reader.GetDouble("proteins"),
                    Fats = reader.GetDouble("fats"),
                    Carbs = reader.GetDouble("carbs")
                });
            }
            return products;
        }

        public static int AddProduct(Product product)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO products (name, calories, proteins, fats, carbs) VALUES (@name, @cal, @prot, @fat, @carb); SELECT LAST_INSERT_ID();",
                conn);
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@cal", product.Calories);
            cmd.Parameters.AddWithValue("@prot", product.Proteins);
            cmd.Parameters.AddWithValue("@fat", product.Fats);
            cmd.Parameters.AddWithValue("@carb", product.Carbs);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static void UpdateProduct(Product product)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "UPDATE products SET name=@name, calories=@cal, proteins=@prot, fats=@fat, carbs=@carb WHERE id=@id",
                conn);
            cmd.Parameters.AddWithValue("@id", product.Id);
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@cal", product.Calories);
            cmd.Parameters.AddWithValue("@prot", product.Proteins);
            cmd.Parameters.AddWithValue("@fat", product.Fats);
            cmd.Parameters.AddWithValue("@carb", product.Carbs);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteProduct(int productId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM products WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", productId);
            cmd.ExecuteNonQuery();
        }

        // ========== Приёмы пищи ==========
        public static List<Meal> GetMealsForDate(DateTime date, int userId)
        {
            var meals = new List<Meal>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            // получаем заголовки приёмов
            var cmdMeals = new MySqlCommand(
                "SELECT id, meal_date, meal_type FROM meals WHERE user_id=@uid AND meal_date=@date",
                conn);
            cmdMeals.Parameters.AddWithValue("@uid", userId);
            cmdMeals.Parameters.AddWithValue("@date", date.Date);
            using var readerMeals = cmdMeals.ExecuteReader();
            while (readerMeals.Read())
            {
                meals.Add(new Meal
                {
                    Id = readerMeals.GetInt32("id"),
                    DateTime = readerMeals.GetDateTime("meal_date"),
                    Type = readerMeals.GetString("meal_type"),
                    Items = new List<MealItem>()
                });
            }
            readerMeals.Close();

            // для каждого приёма загружаем состав
            foreach (var meal in meals)
            {
                var cmdItems = new MySqlCommand(@"
                    SELECT mi.id, mi.quantity_grams,
                           p.id as product_id, p.name, p.calories, p.proteins, p.fats, p.carbs
                    FROM meal_items mi
                    JOIN products p ON mi.product_id = p.id
                    WHERE mi.meal_id = @mealId", conn);
                cmdItems.Parameters.AddWithValue("@mealId", meal.Id);
                using var readerItems = cmdItems.ExecuteReader();
                while (readerItems.Read())
                {
                    var product = new Product
                    {
                        Id = readerItems.GetInt32("product_id"),
                        Name = readerItems.GetString("name"),
                        Calories = readerItems.GetDouble("calories"),
                        Proteins = readerItems.GetDouble("proteins"),
                        Fats = readerItems.GetDouble("fats"),
                        Carbs = readerItems.GetDouble("carbs")
                    };
                    meal.Items.Add(new MealItem
                    {
                        Id = readerItems.GetInt32("id"),
                        Product = product,
                        QuantityGrams = readerItems.GetDouble("quantity_grams")
                    });
                }
            }
            return meals;
        }

        public static List<Meal> GetMealsInRange(int userId, DateTime start, DateTime end)
        {
            var meals = new List<Meal>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmdMeals = new MySqlCommand(
                "SELECT id, meal_date, meal_type FROM meals WHERE user_id=@uid AND meal_date BETWEEN @start AND @end",
                conn);
            cmdMeals.Parameters.AddWithValue("@uid", userId);
            cmdMeals.Parameters.AddWithValue("@start", start.Date);
            cmdMeals.Parameters.AddWithValue("@end", end.Date);
            using var readerMeals = cmdMeals.ExecuteReader();
            while (readerMeals.Read())
            {
                meals.Add(new Meal
                {
                    Id = readerMeals.GetInt32("id"),
                    DateTime = readerMeals.GetDateTime("meal_date"),
                    Type = readerMeals.GetString("meal_type"),
                    Items = new List<MealItem>()
                });
            }
            readerMeals.Close();

            foreach (var meal in meals)
            {
                var cmdItems = new MySqlCommand(@"
                    SELECT mi.id, mi.quantity_grams,
                           p.id as product_id, p.name, p.calories, p.proteins, p.fats, p.carbs
                    FROM meal_items mi
                    JOIN products p ON mi.product_id = p.id
                    WHERE mi.meal_id = @mealId", conn);
                cmdItems.Parameters.AddWithValue("@mealId", meal.Id);
                using var readerItems = cmdItems.ExecuteReader();
                while (readerItems.Read())
                {
                    var product = new Product
                    {
                        Id = readerItems.GetInt32("product_id"),
                        Name = readerItems.GetString("name"),
                        Calories = readerItems.GetDouble("calories"),
                        Proteins = readerItems.GetDouble("proteins"),
                        Fats = readerItems.GetDouble("fats"),
                        Carbs = readerItems.GetDouble("carbs")
                    };
                    meal.Items.Add(new MealItem
                    {
                        Id = readerItems.GetInt32("id"),
                        Product = product,
                        QuantityGrams = readerItems.GetDouble("quantity_grams")
                    });
                }
            }
            return meals;
        }

        public static int AddMeal(Meal meal, int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO meals (user_id, meal_date, meal_type) VALUES (@uid, @date, @type); SELECT LAST_INSERT_ID();",
                conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@date", meal.DateTime.Date);
            cmd.Parameters.AddWithValue("@type", meal.Type);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int AddMealItem(MealItem item, int mealId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO meal_items (meal_id, product_id, quantity_grams) VALUES (@mealId, @prodId, @grams); SELECT LAST_INSERT_ID();",
                conn);
            cmd.Parameters.AddWithValue("@mealId", mealId);
            cmd.Parameters.AddWithValue("@prodId", item.Product.Id);
            cmd.Parameters.AddWithValue("@grams", item.QuantityGrams);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static void DeleteMeal(int mealId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM meals WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", mealId);
            cmd.ExecuteNonQuery();
        }

        // ========== Пользователь и нормы ==========
        public static UserProfile? GetUserProfile(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmdUser = new MySqlCommand(
                "SELECT id, name, age, gender, weight, height, activity_level, goal, password FROM users WHERE id=@id",
                conn);
            cmdUser.Parameters.AddWithValue("@id", userId);
            using var reader = cmdUser.ExecuteReader();
            if (!reader.Read()) return null;

            var user = new UserProfile
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Age = reader.GetInt32("age"),
                Gender = reader.GetString("gender"),
                Weight = reader.GetDouble("weight"),
                Height = reader.GetDouble("height"),
                ActivityFactor = reader.GetDouble("activity_level"),
                Goal = Enum.Parse<GoalType>(reader.GetString("goal")),
                Password = reader.GetString("password")
            };
            reader.Close();

            // загружаем нормы
            var cmdNorms = new MySqlCommand(
                "SELECT daily_calories, protein_goal, fat_goal, carbs_goal FROM user_norms WHERE user_id=@id",
                conn);
            cmdNorms.Parameters.AddWithValue("@id", userId);
            using var readerNorms = cmdNorms.ExecuteReader();
            if (readerNorms.Read())
            {
                user.DailyCalorieNorm = readerNorms.GetDouble("daily_calories");
                user.DailyProteinNorm = readerNorms.GetDouble("protein_goal");
                user.DailyFatNorm = readerNorms.GetDouble("fat_goal");
                user.DailyCarbNorm = readerNorms.GetDouble("carbs_goal");
            }
            else
            {
                RecalculateUserNorm(user); // создаст нормы и сохранит
            }
            return user;
        }

        public static UserProfile CreateDefaultUser()
        {
            var user = new UserProfile
            {
                Name = "Пользователь",
                Age = 30,
                Gender = "Male",
                Weight = 70,
                Height = 170,
                ActivityLevel = "Moderate",
                Goal = GoalType.Maintain
            };

            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(@"
                INSERT INTO users (name, age, gender, weight, height, activity_level, goal)
                VALUES (@name, @age, @gender, @weight, @height, @activity, @goal);
                SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@age", user.Age);
            cmd.Parameters.AddWithValue("@gender", user.Gender);
            cmd.Parameters.AddWithValue("@weight", user.Weight);
            cmd.Parameters.AddWithValue("@height", user.Height);
            cmd.Parameters.AddWithValue("@activity", user.ActivityFactor);
            cmd.Parameters.AddWithValue("@goal", user.Goal.ToString());

            user.Id = Convert.ToInt32(cmd.ExecuteScalar());

            RecalculateUserNorm(user);
            return user;
        }

        public static void SaveUserProfile(UserProfile user)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                UPDATE users SET
                    name=@name, age=@age, gender=@gender, weight=@weight,
                    height=@height, activity_level=@activity, goal=@goal, password=@password
                WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@age", user.Age);
            cmd.Parameters.AddWithValue("@gender", user.Gender);
            cmd.Parameters.AddWithValue("@weight", user.Weight);
            cmd.Parameters.AddWithValue("@height", user.Height);
            cmd.Parameters.AddWithValue("@activity", user.ActivityFactor);
            cmd.Parameters.AddWithValue("@goal", user.Goal.ToString());
            cmd.Parameters.AddWithValue("@password", user.Password);

            cmd.ExecuteNonQuery();

            RecalculateUserNorm(user); // пересчёт и сохранение норм
        }

        public static void RecalculateUserNorm(UserProfile user)
        {
            // Mifflin-St Jeor
            double bmr;
            if (user.Gender == "Male")
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;
            else
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161;

            double multiplier = user.ActivityFactor;   // было: switch по строке
            double tdee = bmr * multiplier;

            double goalFactor = user.Goal switch
            {
                GoalType.Lose => 0.85,
                GoalType.Maintain => 1.0,
                GoalType.Gain => 1.15,
                _ => 1.0
            };
            double targetCalories = tdee * goalFactor;

            user.DailyCalorieNorm = targetCalories;
            user.DailyProteinNorm = (targetCalories * 0.3) / 4;
            user.DailyFatNorm = (targetCalories * 0.2) / 9;
            user.DailyCarbNorm = (targetCalories * 0.5) / 4;
            // Сохраняем в таблицу user_norms
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(@"
                INSERT INTO user_norms (user_id, daily_calories, protein_goal, fat_goal, carbs_goal)
                VALUES (@uid, @cal, @prot, @fat, @carb)
                ON DUPLICATE KEY UPDATE
                    daily_calories = @cal,
                    protein_goal = @prot,
                    fat_goal = @fat,
                    carbs_goal = @carb", conn);
            cmd.Parameters.AddWithValue("@uid", user.Id);
            cmd.Parameters.AddWithValue("@cal", user.DailyCalorieNorm);
            cmd.Parameters.AddWithValue("@prot", user.DailyProteinNorm);
            cmd.Parameters.AddWithValue("@fat", user.DailyFatNorm);
            cmd.Parameters.AddWithValue("@carb", user.DailyCarbNorm);
            cmd.ExecuteNonQuery();
        }

        public static int CreateUser(UserProfile user, string password)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(@"
        INSERT INTO users (name, age, gender, weight, height, activity_level, goal, password)
        VALUES (@name, @age, @gender, @weight, @height, @activity, @goal, @password);
        SELECT LAST_INSERT_ID();", conn);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@age", user.Age);
            cmd.Parameters.AddWithValue("@gender", user.Gender);
            cmd.Parameters.AddWithValue("@weight", user.Weight);
            cmd.Parameters.AddWithValue("@height", user.Height);
            cmd.Parameters.AddWithValue("@activity", user.ActivityFactor);
            cmd.Parameters.AddWithValue("@goal", user.Goal.ToString());
            cmd.Parameters.AddWithValue("@password", password);
            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            user.Id = newId;
            RecalculateUserNorm(user);
            return newId;
        }

        public static void UpdateUserWithPassword(UserProfile user)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(@"
        UPDATE users SET
            name = @name,
            age = @age,
            gender = @gender,
            weight = @weight,
            height = @height,
            activity_level = @activity,
            goal = @goal,
            password = @password
        WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@age", user.Age);
            cmd.Parameters.AddWithValue("@gender", user.Gender);
            cmd.Parameters.AddWithValue("@weight", user.Weight);
            cmd.Parameters.AddWithValue("@height", user.Height);
            cmd.Parameters.AddWithValue("@activity", user.ActivityFactor);
            cmd.Parameters.AddWithValue("@goal", user.Goal.ToString());
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.ExecuteNonQuery();

            // Пересчёт норм
            RecalculateUserNorm(user);
        }

        public static UserProfile? Authenticate(string name, string password)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand(@"
        SELECT id, name, age, gender, weight, height, activity_level, goal
        FROM users WHERE name = @name AND password = @password", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var user = new UserProfile
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Age = reader.GetInt32("age"),
                Gender = reader.GetString("gender"),
                Weight = reader.GetDouble("weight"),
                Height = reader.GetDouble("height"),
                ActivityFactor = reader.GetDouble("activity_level"),
                Goal = Enum.Parse<GoalType>(reader.GetString("goal"))
            };
            reader.Close();

            // Загружаем нормы
            var cmdNorms = new MySqlCommand("SELECT daily_calories, protein_goal, fat_goal, carbs_goal FROM user_norms WHERE user_id=@id", conn);
            cmdNorms.Parameters.AddWithValue("@id", user.Id);
            using var readerNorms = cmdNorms.ExecuteReader();
            if (readerNorms.Read())
            {
                user.DailyCalorieNorm = readerNorms.GetDouble("daily_calories");
                user.DailyProteinNorm = readerNorms.GetDouble("protein_goal");
                user.DailyFatNorm = readerNorms.GetDouble("fat_goal");
                user.DailyCarbNorm = readerNorms.GetDouble("carbs_goal");
            }
            else
            {
                RecalculateUserNorm(user); // на случай отсутствия норм
            }
            return user;
        }

        public static void DeleteMealItem(int mealItemId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM meal_items WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", mealItemId);
            cmd.ExecuteNonQuery();
        }
    }
}
