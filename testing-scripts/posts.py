from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from dotenv import load_dotenv
import os
import time

# Load environment variables from .env file
load_dotenv()

# Get the credentials
username = os.getenv("USERNAME")
password = os.getenv("PASSWORD")

# initialise the driver
driver = webdriver.Firefox()
baseUrl = "https://tosic-blog.azurewebsites.net"
# open the website and login
driver.get(baseUrl + "/Identity/Account/Login")

# wait for page to load
time.sleep(5)

# find the login form and fill it out
username_field = driver.find_element(by="id", value="username")
password_field = driver.find_element(by="id", value="password")

username_field.send_keys(username)
password_field.send_keys(password)

# submit the form
password_field.send_keys(Keys.RETURN)

# navigate to the create blog post page
driver.get(baseUrl + "/BlogPosts/Create")

# wait for page to load
time.sleep(5)

# find the post creation form and fill it out
title_field = driver.find_element(by="id", value="post-title")
content_field = driver.find_element(by="id", value="post-content")

title_field.send_keys("Test Title")
content_field.send_keys("Test Content")

# submit the form
submit_button = driver.find_element(by="id", value="create-post")
submit_button.click()

# wait for page to load
time.sleep(5)

# close the driver
driver.quit()
