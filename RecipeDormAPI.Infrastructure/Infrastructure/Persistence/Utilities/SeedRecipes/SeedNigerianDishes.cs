using Microsoft.Extensions.DependencyInjection;
using RecipeAPI.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Persistence.Utilities.SeedRecipes
{
    public class SeedNigerianDishes
    {
        // Static data for Nigerian dishes (20 each for Yoruba, Igbo, Hausa)
        private static readonly List<(string Category, string Title, string Description, string imageUrl, List<(string Name, string Quantity)> Ingredients, List<(int StepNumber, string Description)> Steps)> NigerianRecipes = new()
        {
            // Yoruba Dishes (20)
            // Main Dishes & Staples (5)
            ("Main Dishes & Staples", "Amala", "Made from yam flour (or cassava flour), usually served with soups like ewedu or gbegiri.", "https://upload.wikimedia.org/wikipedia/commons/thumb/8/86/Amala%2C_ewedu_and_assorted_meat.jpg/300px-Amala%2C_ewedu_and_assorted_meat.jpg",
                new() { ("Yam Flour", "2 cups"), ("Water", "4 cups") },
                new() { (1, "Boil water in a pot."), (2, "Gradually stir in yam flour until thick."), (3, "Cook on low heat for 10 minutes.") }),
            ("Main Dishes & Staples", "Eba", "A stiff dough made from cassava flour (garri), eaten with soups.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRR6ZJKkq9h8DQcq5F-WBw_q6kHSNY5io0v_A&s",
                new() { ("Garri", "2 cups"), ("Water", "3 cups") },
                new() { (1, "Boil water."), (2, "Sprinkle garri into water, stirring until it forms a dough."), (3, "Serve hot.") }),
            ("Main Dishes & Staples", "Iyan (Pounded Yam)", "Smooth, stretchy mashed yam, often paired with egusi or ogbono soup.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRKI9i7LFXhAUfpwbf83JcaGCFqetj2yarXkA&s",
                new() { ("Yam", "1 medium tuber"), ("Water", "As needed") },
                new() { (1, "Peel and boil yam until soft."), (2, "Pound in a mortar until smooth."), (3, "Shape into balls.") }),
            ("Main Dishes & Staples", "Fufu", "Made from cassava or plantain, similar to pounded yam.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSLiVR-KRcz09p8-a08az7CpCdja_Cmj606ag&s",
                new() { ("Cassava", "2 tubers"), ("Water", "As needed") },
                new() { (1, "Peel and boil cassava."), (2, "Pound until smooth."), (3, "Serve with soup.") }),
            ("Main Dishes & Staples", "Ogi (Pap/Akamu)", "Fermented cereal pudding made from maize, millet, or sorghum.", "https://zyfokfoods.com/cdn/shop/products/PHOTO-2023-03-09-21-49-17_1.jpg?v=1681246542",
                new() { ("Maize", "2 cups"), ("Water", "5 cups") },
                new() { (1, "Soak maize for 3 days to ferment."), (2, "Blend and sieve."), (3, "Cook with boiling water until thick.") }),

            // Soups & Stews (5)
            ("Soups & Stews", "Ewedu Soup", "A slimy soup made from jute leaves, usually paired with gbegiri and amala.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTqMVAc1cnSCas-AAyGnwh7uiDo2tdmxb8vfg&s",
                new() { ("Jute Leaves", "2 cups"), ("Locust Beans", "1 tsp"), ("Water", "2 cups") },
                new() { (1, "Blend jute leaves with water."), (2, "Cook with locust beans until slimy."), (3, "Season to taste.") }),
            ("Soups & Stews", "Gbegiri Soup", "A thick bean soup made from peeled beans.", "https://i.ytimg.com/vi/LRaW7qcQDl4/hqdefault.jpg",
                new() { ("Black-Eyed Peas", "2 cups"), ("Palm Oil", "2 tbsp"), ("Pepper", "1 tsp") },
                new() { (1, "Boil peeled beans until soft."), (2, "Mash into a paste."), (3, "Add palm oil and pepper, simmer.") }),
            ("Soups & Stews", "Egusi Soup", "Melon seed-based soup cooked with vegetables, meat, or fish.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRTfojuaO8C2v2szuJB0G5BvKffVqnaoxMn5A&s",
                new() { ("Melon Seeds", "1 cup"), ("Spinach", "2 cups"), ("Beef", "1 lb") },
                new() { (1, "Grind melon seeds."), (2, "Cook beef with spices."), (3, "Add seeds and spinach, simmer.") }),
            ("Soups & Stews", "Ogbono Soup", "A drawy soup made from ground ogbono seeds.", "https://farmcityltd.com/wp-content/uploads/2021/03/ogbono-1-1.jpg",
                new() { ("Ogbono Seeds", "1 cup"), ("Fish", "1 lb"), ("Palm Oil", "3 tbsp") },
                new() { (1, "Grind ogbono seeds."), (2, "Cook fish with palm oil."), (3, "Stir in ogbono until thick.") }),
            ("Soups & Stews", "Efo Riro", "A rich vegetable stew made with spinach or bitter leaf.", "https://sisijemimah.com/wp-content/uploads/2015/06/20190728_121338.jpg",
                new() { ("Spinach", "3 cups"), ("Pepper", "2 tbsp"), ("Assorted Meat", "1 lb") },
                new() { (1, "Cook meat with spices."), (2, "Add pepper and spinach."), (3, "Simmer until tender.") }),

            // Protein-Based Dishes (5)
            ("Protein-Based Dishes", "Moin Moin", "Steamed bean pudding made from blended peeled beans.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSKABYq3Tz4peUxBL_944ya8SW2tecRvL11VU4a4jfN4rl7K2_h9V-UBn8DfWMbGNS_E1k&usqp=CAU",
                new() { ("Black-Eyed Peas", "2 cups"), ("Pepper", "1 tbsp"), ("Fish", "1 piece") },
                new() { (1, "Blend peeled beans with pepper."), (2, "Add fish and steam in leaves."), (3, "Cook for 45 minutes.") }),
            ("Protein-Based Dishes", "Akara", "Deep-fried bean cakes, a popular breakfast or snack.", "https://kikifoodies.com/wp-content/uploads/2024/11/E685E539-B688-4131-BFFE-2288C9899A61-scaled.jpeg",
                new() { ("Black-Eyed Peas", "2 cups"), ("Onion", "1"), ("Oil", "2 cups") },
                new() { (1, "Blend peeled beans with onion."), (2, "Fry spoonfuls in hot oil."), (3, "Drain and serve.") }),
            ("Protein-Based Dishes", "Dodo Ikire", "Fried plantains from Ikire, often caramelized.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQy59IYKRswqATFo-H1kNKnmvGYskqoxbQoCbbsUvDNWNWnBRgUFInPPpXaDyqalX29Ksc&usqp=CAU",
                new() { ("Plantain", "3 ripe"), ("Palm Oil", "1 cup"), ("Sugar", "2 tbsp") },
                new() { (1, "Slice plantains."), (2, "Fry in palm oil with sugar."), (3, "Pack in layers.") }),
            ("Protein-Based Dishes", "Asun", "Spicy roasted goat meat, a popular party snack.", "https://i.ytimg.com/vi/SkNc3TWw5t0/hq720.jpg?sqp=-oaymwEhCK4FEIIDSFryq4qpAxMIARUAAAAAGAElAADIQj0AgKJD&rs=AOn4CLDxtki5xQdQrBRMIoEnDRiYGyliCA",
                new() { ("Goat Meat", "1 lb"), ("Pepper", "2 tbsp"), ("Oil", "2 tbsp") },
                new() { (1, "Roast goat meat."), (2, "Toss with pepper and oil."), (3, "Grill until crispy.") }),
            ("Protein-Based Dishes", "Gizdodo", "A combo of grilled chicken gizzard and fried plantains.", "https://cheflolaskitchen.com/wp-content/uploads/2015/04/Gizdodo-7-scaled.jpg",
                new() { ("Gizzard", "1 lb"), ("Plantain", "2"), ("Pepper", "1 tbsp") },
                new() { (1, "Grill gizzard with pepper."), (2, "Fry plantain slices."), (3, "Mix and serve.") }),

            // Snacks & Side Dishes (5)
            ("Snacks & Side Dishes", "Kokoro", "A crunchy, deep-fried snack made from cornmeal and sugar.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS3yf9n5giSGhR6BIajt1KbghDj4rIBaEMCc3OtWCyUjQlPHi1QF7TDZCAy8F167JvDSaI&usqp=CAU",
                new() { ("Cornmeal", "2 cups"), ("Sugar", "1/2 cup"), ("Oil", "2 cups") },
                new() { (1, "Mix cornmeal with sugar."), (2, "Shape into sticks."), (3, "Fry until golden.") }),
            ("Snacks & Side Dishes", "Ekuru", "Steamed bean cakes similar to moin moin but plainer.", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTkJ-QbXMDy8ozLm0IEF9kgLdW4zNKtKWoSHKXCihwlnUBHkqfW7aAtQCRlbwLVpQ7gzo0&usqp=CAU",
                new() { ("Black-Eyed Peas", "2 cups"), ("Pepper", "1 tsp"), ("Water", "1 cup") },
                new() { (1, "Blend peeled beans."), (2, "Steam with pepper."), (3, "Serve with sauce.") }),
            ("Snacks & Side Dishes", "Ojojo", "Grated water yam fritters, spiced and deep-fried.", "https://img-global.cpcdn.com/recipes/dc5b53e4b3ee7efc/680x482cq70/water-yam-cake-ojojo-recipe-main-photo.jpg",
                new() { ("Water Yam", "1 tuber"), ("Pepper", "1 tsp"), ("Oil", "2 cups") },
                new() { (1, "Grate water yam."), (2, "Mix with pepper and fry."), (3, "Drain excess oil.") }),
            ("Snacks & Side Dishes", "Ewa Agoyin", "Boiled beans served with a spicy pepper sauce.", "https://i.ytimg.com/vi/QGhE1Y5UKCE/hq720.jpg?sqp=-oaymwEhCK4FEIIDSFryq4qpAxMIARUAAAAAGAElAADIQj0AgKJD&rs=AOn4CLCx5Vh-Y_wfeNYHGwya7HfAjv4xjg",
                new() { ("Honey Beans", "2 cups"), ("Pepper", "2 tbsp"), ("Palm Oil", "1/2 cup") },
                new() { (1, "Boil beans until soft."), (2, "Cook pepper in palm oil."), (3, "Serve sauce over beans.") }),
            ("Snacks & Side Dishes", "Puff-Puff", "Deep-fried dough balls, a popular Yoruba snack.", "https://simshomekitchen.com/wp-content/uploads/2023/01/puffpuff-nigerian.jpg",
                new() { ("Flour", "2 cups"), ("Sugar", "1/2 cup"), ("Yeast", "1 tsp") },
                new() { (1, "Mix flour, sugar, and yeast with water."), (2, "Let dough rise."), (3, "Fry in balls.") }),

            // Igbo Dishes (20)
            // Main Dishes & Staples (5)
            ("Main Dishes & Staples", "Fufu (Akpu)", "Fermented cassava dough.", "https://i.pinimg.com/originals/98/d8/cd/98d8cd4ef996cebd6a51184d3b6f9e7e.jpg",
                new() { ("Cassava", "2 tubers"), ("Water", "As needed") },
                new() { (1, "Ferment cassava for 3 days."), (2, "Boil and pound."), (3, "Serve with soup.") }),
            ("Main Dishes & Staples", "Garri (Eba)", "Cassava flakes made into a dough, eaten with soups.", "https://th.bing.com/th/id/OIP.zZ0xGhjKcxbJqMl6-fhFtQHaHO?rs=1&pid=ImgDetMain",
                new() { ("Garri", "2 cups"), ("Water", "3 cups") },
                new() { (1, "Boil water."), (2, "Stir in garri until thick."), (3, "Shape and serve.") }),
            ("Main Dishes & Staples", "Pounded Yam (Ji Akpụkụ)", "Smooth yam mash, paired with soups.", "https://desirerecipes.com/wp-content/uploads/2022/02/image-24.jpg",
                new() { ("Yam", "1 tuber"), ("Water", "As needed") },
                new() { (1, "Boil yam until soft."), (2, "Pound until stretchy."), (3, "Serve hot.") }),
            ("Main Dishes & Staples", "Abacha (African Salad)", "Shredded cassava mixed with ugba and spices.", "https://www.nairaland.com/attachments/11675405_abacharecipe_jpeg0ee36b0360597450027c1bdf1b842a3f",
                new() { ("Cassava", "2 cups"), ("Ugba", "1/2 cup"), ("Palm Oil", "2 tbsp") },
                new() { (1, "Soak cassava shreds."), (2, "Mix with ugba and palm oil."), (3, "Season and serve.") }),
            ("Main Dishes & Staples", "Ukwa (Breadfruit Porridge)", "Cooked breadfruit seeds, a hearty main dish.", "https://th.bing.com/th/id/OIP.5XX5mxw7bX09wea9yO50pAHaEK?w=580&h=326&rs=1&pid=ImgDetMain",
                new() { ("Breadfruit Seeds", "2 cups"), ("Palm Oil", "2 tbsp"), ("Fish", "1 piece") },
                new() { (1, "Boil breadfruit seeds."), (2, "Add palm oil and fish."), (3, "Simmer until tender.") }),

            // Soups & Stews (5)
            ("Soups & Stews", "Ofe Onugbu (Bitter Leaf Soup)", "A slightly bitter soup with meat and fish.", "https://th.bing.com/th/id/OIP.LZCn8k1P6Ed_0QTUZ2Tn8AHaHa?rs=1&pid=ImgDetMain",
                new() { ("Bitter Leaves", "2 cups"), ("Assorted Meat", "1 lb"), ("Fish", "1 lb") },
                new() { (1, "Cook meat and fish."), (2, "Add washed bitter leaves."), (3, "Simmer until flavorful.") }),
            ("Soups & Stews", "Ofe Oha (Ora Soup)", "Thick soup made with oha leaves and cocoyam.", "https://www.mydiasporakitchen.com/wp-content/uploads/2017/08/img_9640-1.jpg",
                new() { ("Oha Leaves", "2 cups"), ("Cocoyam", "5 pieces"), ("Stockfish", "1 piece") },
                new() { (1, "Boil and mash cocoyam."), (2, "Add stockfish and oha leaves."), (3, "Cook until thick.") }),
            ("Soups & Stews", "Ofe Nsala (White Soup)", "A light, spicy soup without palm oil.", "https://th.bing.com/th/id/R.78b029c280fc77642c854e1f62167b51?rik=r21flYBQ482DPg&pid=ImgRaw&r=0",
                new() { ("Catfish", "1 lb"), ("Uziza Leaves", "1 tsp"), ("Pepper", "1 tbsp") },
                new() { (1, "Cook catfish with pepper."), (2, "Add uziza leaves."), (3, "Simmer lightly.") }),
            ("Soups & Stews", "Ofe Akwu (Palm Nut Soup)", "Palm nut soup often eaten with rice or fufu.", "https://th.bing.com/th/id/OIP.XolroI4qWduxkZ-ed4fhFQHaE7?w=288&h=191&c=7&r=0&o=5&pid=1.7",
                new() { ("Palm Nuts", "2 cups"), ("Fish", "1 lb"), ("Pepper", "1 tbsp") },
                new() { (1, "Boil palm nuts and extract juice."), (2, "Cook with fish and pepper."), (3, "Simmer until rich.") }),
            ("Soups & Stews", "Ofe Egusi", "Thick melon seed soup with vegetables.", "https://th.bing.com/th/id/R.ff5bd3e40c9bd2cec8e53a00ee182234?rik=7o%2fz9RulXiVGUg&pid=ImgRaw&r=0",
                new() { ("Melon Seeds", "1 cup"), ("Vegetables", "2 cups"), ("Meat", "1 lb") },
                new() { (1, "Grind melon seeds."), (2, "Cook meat with spices."), (3, "Add seeds and veggies.") }),

            // Protein-Based Dishes (5)
            ("Protein-Based Dishes", "Okpa", "Steamed pudding made from Bambara nut flour.", "https://healthguide.ng/wp-content/uploads/2019/12/IMG_20191223_011313_140.jpg",
                new() { ("Bambara Nut Flour", "2 cups"), ("Palm Oil", "2 tbsp"), ("Pepper", "1 tsp") },
                new() { (1, "Mix flour with palm oil and pepper."), (2, "Steam in leaves."), (3, "Cook for 1 hour.") }),
            ("Protein-Based Dishes", "Moin Moin Igbo (Jigbo)", "Bean pudding with crayfish and fish.", "https://th.bing.com/th/id/R.9c596005ac3dd4d284326ef3aeb41c7c?rik=nYMOBKsioElclA&pid=ImgRaw&r=0",
                new() { ("Beans", "2 cups"), ("Crayfish", "2 tbsp"), ("Fish", "1 piece") },
                new() { (1, "Blend beans with crayfish."), (2, "Add fish and steam."), (3, "Serve hot.") }),
            ("Protein-Based Dishes", "Nkwobi", "Spicy cow foot in palm oil and utazi leaves.", "https://th.bing.com/th/id/R.88eba9f203b28b86ab8db9f1deeff0b9?rik=ZdBgWMpUh65%2fQQ&pid=ImgRaw&r=0",
                new() { ("Cow Foot", "1 lb"), ("Palm Oil", "2 tbsp"), ("Utazi Leaves", "1 tsp") },
                new() { (1, "Cook cow foot until tender."), (2, "Mix with palm oil and utazi."), (3, "Serve warm.") }),
            ("Protein-Based Dishes", "Isi Ewu", "Spicy goat head pepper soup with palm oil.", "https://th.bing.com/th/id/OIP.PXVBK-9rJXHJDw6IYx5N4gHaJ4?rs=1&pid=ImgDetMain",
                new() { ("Goat Head", "1"), ("Palm Oil", "3 tbsp"), ("Pepper", "2 tbsp") },
                new() { (1, "Cook goat head with spices."), (2, "Add palm oil and pepper."), (3, "Simmer until rich.") }),

            // Snacks & Side Dishes (5)
            ("Snacks & Side Dishes", "Akara", "Deep-fried bean balls with Igbo spices.", "https://th.bing.com/th/id/R.446419ba0a33bd9c658c3401494270df?rik=3Y84%2fQUWvSj8EA&pid=ImgRaw&r=0",
                new() { ("Beans", "2 cups"), ("Onion", "1"), ("Pepper", "1 tsp") },
                new() { (1, "Blend beans with onion and pepper."), (2, "Fry in hot oil."), (3, "Drain and serve.") }),
            ("Snacks & Side Dishes", "Ugba (Ukpaka)", "Fermented oil bean seeds, used in abacha.", "https://i.etsystatic.com/25608563/r/il/bdf5b5/2659540271/il_794xN.2659540271_aa8a.jpg",
                new() { ("Oil Beans", "1 cup"), ("Salt", "1 tsp"), ("Palm Oil", "1 tbsp") },
                new() { (1, "Ferment oil beans for 3 days."), (2, "Mix with salt and palm oil."), (3, "Serve as side.") }),
            ("Snacks & Side Dishes", "Agidi", "Corn pudding paired with soups or stews.", "https://th.bing.com/th/id/OIP.e2GASklXaUrktUlEDv9uIAHaFj?rs=1&pid=ImgDetMain",
                new() { ("Cornmeal", "2 cups"), ("Water", "3 cups") },
                new() { (1, "Mix cornmeal with water."), (2, "Cook until thick."), (3, "Cool and cut.") }),
            ("Snacks & Side Dishes", "Ji Mmanụ", "Fried yam slices in palm oil.", "https://i.pinimg.com/originals/4b/b6/0e/4bb60e48f32e57d0c7745a18d854adca.jpg ",
                new() { ("Yam", "1 tuber"), ("Palm Oil", "1/2 cup"), ("Salt", "1 tsp") },
                new() { (1, "Slice yam thinly."), (2, "Fry in palm oil with salt."), (3, "Serve crispy.") }),
            ("Snacks & Side Dishes", "Oka na Ose", "Spiced cornmeal with coconut sauce.", "https://th.bing.com/th/id/OIP.jTjkkLYGSAa7XV-fAez-CQHaE8?rs=1&pid=ImgDetMain",
                new() { ("Cornmeal", "2 cups"), ("Coconut Milk", "1 cup"), ("Pepper", "1 tsp") },
                new() { (1, "Cook cornmeal with water."), (2, "Prepare coconut sauce with pepper."), (3, "Serve together.") }),

            // Hausa Dishes (20)
            // Main Dishes & Staples (5)
            ("Main Dishes & Staples", "Tuwo Shinkafa", "Thick rice pudding-like dish, served with soups.", "https://eatwellabi.com/wp-content/uploads/2022/10/Jamaican-chicken-soup-and-tuwo-17.jpg",
                new() { ("Rice", "2 cups"), ("Water", "4 cups") },
                new() { (1, "Boil rice until soft."), (2, "Mash into a thick paste."), (3, "Serve with soup.") }),
            ("Main Dishes & Staples", "Tuwo Masara", "Stiff cornmeal porridge, a Hausa staple.", "https://th.bing.com/th/id/R.c2cc7f5f9542ba75a4ab53cc46f0c042?rik=DsjKlpIxj2toMw&riu=http%3a%2f%2f4.bp.blogspot.com%2f_GKA46Z26UVo%2fSn6-WX26EeI%2fAAAAAAAAAQ8%2f_lmP4x63nuQ%2fs320%2f023.JPG&ehk=ovKbfJr%2flD26lJaPAm5mCRouG%2bXbhj1qIM4jGHsSl9o%3d&risl=&pid=ImgRaw&r=0",
                new() { ("Cornmeal", "2 cups"), ("Water", "3 cups") },
                new() { (1, "Boil water."), (2, "Stir in cornmeal until thick."), (3, "Shape and serve.") }),
            ("Main Dishes & Staples", "Tuwo Dawa", "Millet-based swallow, common in Hausa homes.", "https://afrifoodnetwork.com/wp-content/uploads/2021/09/E3D56CDF-9234-47E2-AB61-36482DDADFB4-scaled.jpeg",
                new() { ("Millet", "2 cups"), ("Water", "4 cups") },
                new() { (1, "Cook millet with water."), (2, "Stir until thick."), (3, "Serve with stew.") }),
            ("Main Dishes & Staples", "Dan Wake", "Bean dumplings served with pepper sauce or yogurt.", "https://i2.wp.com/www.afrolems.com/wp-content/uploads/2020/04/how-to-make-glazed-danwake-scaled.jpg?w=1920",
                new() { ("Beans", "1 cup"), ("Flour", "1 cup"), ("Pepper Sauce", "1/2 cup") },
                new() { (1, "Mix beans and flour into dough."), (2, "Boil dumplings."), (3, "Serve with sauce.") }),
            ("Main Dishes & Staples", "Dambu Nama", "Shredded, spiced dried meat, eaten with rice.", "https://th.bing.com/th/id/OIP.MCNPtHJdq-UtRJKy31vf5QHaHa?rs=1&pid=ImgDetMain",
                new() { ("Beef", "1 lb"), ("Spices", "2 tbsp"), ("Salt", "1 tsp") },
                new() { (1, "Cook beef with spices."), (2, "Shred and dry."), (3, "Serve as topping.") }),

            // Soups & Stews (5)
            ("Soups & Stews", "Miyan Kuka", "Tangy soup from powdered baobab leaves.", "https://th.bing.com/th/id/R.833403e27127300317ac1e41881fb2de?rik=Fat6D23hfzDLJA&riu=http%3a%2f%2fwww.gratednutmeg.com%2fwp-content%2fuploads%2f2014%2f07%2fKuka-Soup.png&ehk=5Xtkjg7X%2fkgmZCgZvsAOPDLhySnmBP35rT9Bgc%2fDywQ%3d&risl=&pid=ImgRaw&r=0",
                new() { ("Baobab Leaves", "1 cup"), ("Dried Fish", "1 lb"), ("Pepper", "1 tbsp") },
                new() { (1, "Cook fish with pepper."), (2, "Add baobab powder."), (3, "Simmer until thick.") }),
            ("Soups & Stews", "Miyan Taushe", "Pumpkin soup with groundnuts and spinach.", "https://eatwellabi.com/wp-content/uploads/2021/12/miyan-taushe-17.jpg",
                new() { ("Pumpkin", "2 cups"), ("Groundnuts", "1 cup"), ("Spinach", "1 cup") },
                new() { (1, "Boil pumpkin and groundnuts."), (2, "Add spinach."), (3, "Season and simmer.") }),
            ("Soups & Stews", "Miyan Kubewa", "Dried okra soup with meat or fish.", "https://th.bing.com/th/id/R.fa408c7b112bbb70f69ca7ec99879549?rik=ls5G2ZXDgkhluQ&riu=http%3a%2f%2fwww.gratednutmeg.com%2fwp-content%2fuploads%2f2014%2f04%2fOkro.png&ehk=bmj%2f%2fFQSFhpRpRsIFqciz3EaKIYDIg81KcsAmGT8Stk%3d&risl=&pid=ImgRaw&r=0",
                new() { ("Dried Okra", "1 cup"), ("Meat", "1 lb"), ("Pepper", "1 tbsp") },
                new() { (1, "Cook meat with pepper."), (2, "Add dried okra."), (3, "Simmer until thick.") }),
            ("Soups & Stews", "Miyan Zogale", "Moringa leaf soup, rich in nutrients.", "https://img-global.cpcdn.com/recipes/a7faf846c382b86a/680x482cq70/tuwo-miyan-zogale-recipe-main-photo.jpg",
                new() { ("Moringa Leaves", "2 cups"), ("Fish", "1 lb"), ("Groundnuts", "1/2 cup") },
                new() { (1, "Cook fish with groundnuts."), (2, "Add moringa leaves."), (3, "Simmer gently.") }),
            ("Soups & Stews", "Miyan Gyada", "Groundnut soup with a Hausa twist.", "https://th.bing.com/th/id/R.7443d4fac820b6a03ee6591cb69ade44?rik=T5AG%2fe18beag6A&riu=http%3a%2f%2fwww.gratednutmeg.com%2fwp-content%2fuploads%2f2014%2f10%2f11693889_10153189015182763_4476397135279401312_n.jpg&ehk=21i1CaDyA2%2fWOWVukLWvzA1AI%2bR51tsQ9jcRiFKuqMI%3d&risl=&pid=ImgRaw&r=0 ",
                new() { ("Groundnuts", "1 cup"), ("Meat", "1 lb"), ("Pepper", "1 tbsp") },
                new() { (1, "Grind groundnuts."), (2, "Cook meat with pepper."), (3, "Add groundnuts and simmer.") }),

            // Protein-Based Dishes (5)
            ("Protein-Based Dishes", "Suya", "Skewered, spicy grilled meat coated in yaji spice.", "https://guardian.ng/wp-content/uploads/2019/09/Suya-Samis-Online-Store-1728x1152.jpg",
                new() { ("Beef", "1 lb"), ("Yaji Spice", "2 tbsp"), ("Oil", "1 tbsp") },
                new() { (1, "Marinate beef with yaji."), (2, "Skewer and grill."), (3, "Serve hot.") }),
            ("Protein-Based Dishes", "Kilishi", "Spiced, dried meat similar to jerky.", "https://kikiisbite.com.ng/wp-content/uploads/2020/11/Sweet-Spicy-Kilishi_by-Majestell.jpg",
                new() { ("Beef", "1 lb"), ("Spices", "2 tbsp"), ("Salt", "1 tsp") },
                new() { (1, "Slice beef thinly."), (2, "Season and dry in sun."), (3, "Grill lightly.") }),
            ("Protein-Based Dishes", "Balangu", "Grilled ram or goat meat, lightly spiced.", "https://c1.staticflickr.com/1/777/32607661611_f6a4d2d3fa_c.jpg",
                new() { ("Ram Meat", "1 lb"), ("Pepper", "1 tbsp"), ("Salt", "1 tsp") },
                new() { (1, "Season ram meat."), (2, "Grill over fire."), (3, "Serve smoky.") }),
            ("Protein-Based Dishes", "Fura da Nono", "Millet balls eaten with fermented milk.", "https://th.bing.com/th/id/OIP.Jdoq_Fkwt8nA3oKhgid44gAAAA?rs=1&pid=ImgDetMain",
                new() { ("Millet", "2 cups"), ("Spices", "1 tsp"), ("Nono", "1 cup") },
                new() { (1, "Cook millet into balls."), (2, "Mix with spices."), (3, "Serve with nono.") }),
            ("Protein-Based Dishes", "Kosai", "Deep-fried bean cakes, Hausa-style.", "https://i.ytimg.com/vi/qatN7l7-3FA/maxresdefault.jpg",
                new() { ("Beans", "2 cups"), ("Onion", "1"), ("Pepper", "1 tsp") },
                new() { (1, "Blend beans with onion."), (2, "Fry with pepper."), (3, "Drain and serve.") }),

            // Snacks & Side Dishes (5)
            ("Snacks & Side Dishes", "Masa (Waina)", "Fermented rice cakes, slightly sweet.", "https://img-global.cpcdn.com/recipes/cd3bf673345ea8a4/680x482cq70/masa-aka-waina-recipe-main-photo.jpg",
                new() { ("Rice", "2 cups"), ("Yeast", "1 tsp"), ("Sugar", "2 tbsp") },
                new() { (1, "Ferment rice with yeast."), (2, "Cook in small molds."), (3, "Serve with honey.") }),
            ("Snacks & Side Dishes", "Alkubus", "Steamed bread, softer than agege bread.", "https://cdn.tasteatlas.com/images/dishes/ae032422ec2a4390a1c8075503ff263f.jpg",
                new() { ("Flour", "2 cups"), ("Yeast", "1 tsp"), ("Water", "1 cup") },
                new() { (1, "Mix flour with yeast and water."), (2, "Steam until fluffy."), (3, "Serve warm.") }),
            ("Snacks & Side Dishes", "Gurasa", "Thick, pancake-like bread from wheat flour.", "https://img-global.cpcdn.com/recipes/0be7b86a89455b9b/680x482cq70/pan-milky-gurasa-recipe-main-photo.jpg",
                new() { ("Wheat Flour", "2 cups"), ("Water", "1 cup"), ("Salt", "1 tsp") },
                new() { (1, "Mix flour with water and salt."), (2, "Grill until golden."), (3, "Serve with soup.") }),
            ("Snacks & Side Dishes", "Dakushe", "Sweet snack from groundnuts and honey.", "https://th.bing.com/th/id/OIP.WPEJzhHK-9eE3fwGzqLiKwHaEK?rs=1&pid=ImgDetMain",
                new() { ("Groundnuts", "1 cup"), ("Honey", "1/2 cup"), ("Coconut", "2 tbsp") },
                new() { (1, "Grind groundnuts."), (2, "Mix with honey and coconut."), (3, "Shape and dry.") }),
            ("Snacks & Side Dishes", "Koko (Kunun Tsamiya)", "Spicy millet porridge for breakfast.", "https://th.bing.com/th/id/OIP.O68p_mWX6hhjffvQd3ZuZgHaEc?w=271&h=180&c=7&r=0&o=5&pid=1.7",
                new() { ("Millet", "2 cups"), ("Tamarind", "1 tbsp"), ("Pepper", "1 tsp") },
                new() { (1, "Cook millet with water."), (2, "Add tamarind and pepper."), (3, "Serve warm.") })
        };

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _dataDbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();

            var existingTitles = _dataDbContext.Recipes.Select(r => r.Title).ToHashSet();
            //var existingTitles = await _dataDbContext.Recipes.Select(r => r.Title).ToHashSetAsync();
            //var existingTitles = new HashSet<string>(await _dataDbContext.Recipes.Select(r => r.Title).ToListAsync());

            try
            {
                foreach (var (category, title, description, imageUrl, ingredients, steps) in NigerianRecipes)
                {
                    if (existingTitles.Contains(title))
                    {
                        Console.WriteLine($"Duplicate '{title}' skipped.");
                        continue;
                    }

                    var recipe = new Recipes
                    {
                        Id = Guid.NewGuid(),
                        UserId = null, // Excluded as manually seeded by devs
                        Title = title,
                        Description = description,
                        SpoonacularId = -1, // Custom ID for manual seeding (-1 as placeholder)
                        //ImageUrl = $"https://placeholder.com/nigerian-{title.ToLower().Replace(" ", "-")}.jpg", 
                        ImageUrl = imageUrl,
                        Ingredients = ingredients.Select(ing => new Ingredients
                        {
                            Id = Guid.NewGuid(),
                            RecipeId = Guid.Empty, // Set after adding to DB
                            Name = ing.Name,
                            Quantity = ing.Quantity
                        }).ToList(),
                        Steps = steps.Select(step => new Steps
                        {
                            Id = Guid.NewGuid(),
                            RecipeId = Guid.Empty, // Set after adding to DB
                            StepNumber = step.StepNumber,
                            Description = step.Description
                        }).ToList()
                    };

                    // Set RecipeId for child entities
                    recipe.Ingredients.ForEach(i => i.RecipeId = recipe.Id);
                    recipe.Steps.ForEach(s => s.RecipeId = recipe.Id);

                    _dataDbContext.Recipes.Add(recipe);
                    existingTitles.Add(title);
                }

                await _dataDbContext.SaveChangesAsync();
                Console.WriteLine($"Seeded {NigerianRecipes.Count} Nigerian dishes (20 Yoruba, 20 Igbo, 20 Hausa)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding failed: {ex.Message}");
                throw; // Re-throw for debugging or logging in calling code
            }
        }
    }
}
